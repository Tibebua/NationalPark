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
    
    [Route("api/v{version:apiVersion}/Trails")]
    [ApiController]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var trails = _trailRepo.GetTrails();
            var objDto = new List<TrailDto>();

            foreach (var obj in trails)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }

        [HttpGet("GetTrailsInaNationalPark/{nationalParkId}")]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrailsInNationalPark(int nationalParkId)
        {
            var objList = _trailRepo.GetTrailsInaNationalPark(nationalParkId);
            var trailListDto = new List<TrailDto>();

            if(objList == null)
            {
                return NotFound();
            }

            foreach (var obj in objList)
            {
                trailListDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(trailListDto);
        }

        /// <summary>
        /// Gets a single trail
        /// </summary>
        /// <param name="trailId"></param>
        /// <returns></returns>
        [HttpGet("{trailId}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {
            var trail = _trailRepo.GetTrail(trailId);
            if (trail == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TrailDto>(trail));
        }

        /// <summary>
        /// Create a trail
        /// </summary>
        /// <remarks>
        /// sample request: 
        ///  POST /Trails
        ///  {
        ///     "Name": "Harer national trail",
        ///     "State": "Harer"
        /// }
        /// </remarks>
        /// <param name="trailDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }
            var objToBeCreated = _mapper.Map<Trail>(trailDto);
            if (!_trailRepo.CreateTrail(objToBeCreated))
            {
                ModelState.AddModelError("", $"something went wrong creating the record {objToBeCreated.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new { trailId = objToBeCreated.Id }, objToBeCreated);
        }

        [HttpPatch("{trailId}", Name = "UpdateTrail")]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null || trailId != trailDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_trailRepo.TrailExists(trailId))
            {
                ModelState.AddModelError("", "Trail Does Not Exist!");
                return StatusCode(404, ModelState);
            }
            var objToBeUpdated = _mapper.Map<Trail>(trailDto);
            if (!_trailRepo.UpdateTrail(objToBeUpdated))
            {
                ModelState.AddModelError("", $"something went wrong updating the record {objToBeUpdated.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific entity by Id
        /// </summary>
        /// <param name="trailId"></param>
        /// <returns></returns>
        [HttpDelete("{trailId}", Name = "DeleteTrail")]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepo.TrailExists(trailId))
            {
                return NotFound();
            }

            var trailToBeDeleted = _trailRepo.GetTrail(trailId);
            if (!_trailRepo.DeleteTrail(trailToBeDeleted))
            {
                ModelState.AddModelError("", $"something went wrong Deleting the record {trailToBeDeleted.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


    }
}
