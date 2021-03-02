using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SD
    {
        public static string APIBaseUrl = "http://localhost:44323/";
        public static string NationalParkAPIPath = APIBaseUrl + "api/v1/nationalParks/";
        public static string TrailAPIPath = APIBaseUrl + "api/v1/Trails/";
    }
}
