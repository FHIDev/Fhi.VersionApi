﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace VersionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VersionApiControllers : ControllerBase
    {
        static string filename = "versionApiFile.json";
        static Dictionary<string, VersionDTO> information = new();

        //static VersionApiControllers()
        //{
        //    information = ReadDictonary();
        //}

        private static Dictionary<string, VersionDTO> ReadDictonary()
        {
            if (!System.IO.File.Exists(filename))
            {
                return new Dictionary<string, VersionDTO>();

            }

            string allText = System.IO.File.ReadAllText(filename);
            var versionApiDict = JsonConvert.DeserializeObject<Dictionary<string, VersionDTO>>(allText);

            return versionApiDict!;
       
        }

        private string CreateKey(string enviroment, string system, string component) => $"{enviroment}.{system}.{component}";


        [HttpGet("UploadInformation")]
        public void UploadInformation()
        {
            string jsonString = JsonConvert.SerializeObject(information, Formatting.Indented);
            System.IO.File.WriteAllText(filename, jsonString);
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
        public void SetInformation(string enviroment, string system, string component, string version)
        {
            VersionDTO dto = new()
            {
                Enviroment = enviroment,
                System = system,
                Component = component,
                Version = version
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

            // UploadInformation();
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
    }

    /// <summary>
    /// To be used with Badges in Shields.io, ref https://shields.io/endpoint
    /// </summary>
    public class ShieldsIo
    {
        public int schemaVersion => 1;
        public string label { get; set; }
        public string message { get; set; }

        public string color { get; set; } = "lightgrey";

        public ShieldsIo(string label, string message)
        {
            this.label = label;
            this.message = message;
        }
    }

}

