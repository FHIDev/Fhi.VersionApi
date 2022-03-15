﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace VersionApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class VersionApiControllers : ControllerBase
    {
        static readonly Dictionary<string, VersionDTO> information;
        private static string path = "";

        static VersionApiControllers()
        {
            information = ReadDictonary();
        }

        private static Dictionary<string, VersionDTO> ReadDictonary()
        {
            const string filename = "versionApiFile.json";
            var folder = Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            path = Path.Combine(folder, filename);
            if (!System.IO.File.Exists(path))
            {
                return new Dictionary<string, VersionDTO>();

            }

            string allText = System.IO.File.ReadAllText(path);
            var versionApiDict = JsonConvert.DeserializeObject<Dictionary<string, VersionDTO>>(allText);

            return versionApiDict!;

        }

        private string CreateKey(string enviroment, string system, string component) => $"{enviroment}.{system}.{component}";


        [HttpGet("UploadInformation")]
        public void UploadInformation()
        {
            string jsonString = JsonConvert.SerializeObject(information, Formatting.Indented);
            System.IO.File.WriteAllText(path, jsonString);
        }

        [HttpGet("GetInformation")]
        public ActionResult<ShieldsIo> GetInformation(string enviroment, string system, string component)
        {
            var dtoFound = information.TryGetValue(CreateKey(enviroment, system, component), out var dto);

            if (dtoFound == false)
            {
                return Ok(new ShieldsIo("Version", "Not Found"));
            }
            else
            {
                return Ok(new ShieldsIo("Version", dto!.Version));
            }

        }

        [HttpGet("SetInformation")]
        public void SetInformation(string enviroment, string system, string component, string version, string status)
        {
            VersionDTO dto = new()
            {
                Enviroment = enviroment,
                System = system,
                Component = component,
                Version = version,
                Status = status
            };

            string key = CreateKey(enviroment, system, component);

            var dtoFound = information.TryGetValue(key, out var outdto);
            if (!dtoFound)
            {
                information.Add(key, dto);
            }
            else
            {
                information[key] = dto;
            }

            UploadInformation();
        }

        [HttpGet("DeleteInformation")]
        public void DeleteInformation(string system, string component)
        {
            var dtoFound = information.TryGetValue($"{system}.{component}", out var dto);

            if (dtoFound == false)
            {
                return;
            }
            else
            {
                information.Remove($"{system}.{component}");
            }
        }

        [HttpGet("DeleteAllInformation")]
        public void DeleteAllInformation()
        {
            information.Clear();
        }

        [HttpGet("Status")]
        public IActionResult StatusImage(int num)
        {
            Byte[] image = Array.Empty<byte>();

            image = num switch
            {
                0 => System.IO.File.ReadAllBytes("images/reddot.png"),
                1 => System.IO.File.ReadAllBytes("images/greendot.png"),
                2 => System.IO.File.ReadAllBytes("images/yellowdot.png"),
                3 => System.IO.File.ReadAllBytes("images/orangedot.png"),
                _ => System.IO.File.ReadAllBytes("images/questionmark.png"),
            };

            return File(image, "image/jpeg");

        }

        [HttpGet("StatusText")]
        public IActionResult StatusText(string text)
        {
            string newtext = text.ToLower();
            Byte[] image = Array.Empty<byte>();

            image = newtext switch
            {
                "unhealthy" or "error" => System.IO.File.ReadAllBytes("images/reddot.png"),
                "healthy" => System.IO.File.ReadAllBytes("images/greendot.png"),
                "warning" => System.IO.File.ReadAllBytes("images/yellowdot.png"),
                "degraded" => System.IO.File.ReadAllBytes("images/orangedot.png"),
                _ => System.IO.File.ReadAllBytes("images/questionmark.png"),
            };

            return File(image, "image/jpeg");
        }

        [HttpGet("HealthStatus")]
        public IActionResult HealthStatus(Status status)
        {
            return StatusText(status.OverStatus2);
        }


    }

}

