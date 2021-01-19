using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Dtos;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(List<NationalParkDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetNationalParks()
        {
            var parks = _npRepo.GetNationalParks();
            return Ok(_mapper.Map<ICollection<NationalPark>, ICollection<NationalParkDto>>(parks));
        }



        /// <summary>
        /// Gets a single national park
        /// </summary>
        /// <param name="nationalParkId"></param>
        /// <returns></returns>
        [HttpGet("{nationalParkId}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(404)]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var park = _npRepo.GetNationalPark(nationalParkId);
            if (park == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<NationalPark, NationalParkDto>(park));
        }

        /// <summary>
        /// Create a national park
        /// </summary>
        /// <remarks>
        /// sample request: 
        ///  POST /NationalParks
        ///  {
        ///     "Name": "Harer national park",
        ///     "State": "Harer"
        /// }
        /// </remarks>
        /// <param name="nationalParkDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }
            var objToBeCreated = _mapper.Map<NationalParkDto, NationalPark>(nationalParkDto);
            if (!_npRepo.CreateNationalPark(objToBeCreated))
            {
                ModelState.AddModelError("", $"something went wrong creating the record {objToBeCreated.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationalPark", new { nationalParkId = objToBeCreated.Id }, objToBeCreated);
        }

        [HttpPatch("{natId}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int natId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || natId != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_npRepo.NationalParkExists(natId))
            {
                ModelState.AddModelError("", "National Park Does Not Exist!");
                return StatusCode(404, ModelState);
            }
            var objToBeUpdated = _mapper.Map<NationalParkDto, NationalPark>(nationalParkDto);
            if (!_npRepo.UpdateNationalPark(objToBeUpdated))
            {
                ModelState.AddModelError("", $"something went wrong updating the record {objToBeUpdated.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific entity by Id
        /// </summary>
        /// <param name="natId"></param>
        /// <returns></returns>
        [HttpDelete("{natId}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNationalPark(int natId)
        {
            if (!_npRepo.NationalParkExists(natId))
            {
                return NotFound();
            }

            var nationalParkToBeDeleted = _npRepo.GetNationalPark(natId);
            if (!_npRepo.DeleteNationalPark(nationalParkToBeDeleted))
            {
                ModelState.AddModelError("", $"something went wrong Deleting the record {nationalParkToBeDeleted.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


    }
}
