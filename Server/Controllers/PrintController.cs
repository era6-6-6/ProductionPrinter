using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintController : ControllerBase
    {
        private readonly Dictionary<string, string> _printerAddresses = new Dictionary<string, string>
        {
            { "Vstrikovna", "192.168.1.100" },
            { "Lakovna", "192.168.1.101" },
            { "Montaz", "192.168.1.102" }
        };
        [HttpGet("preview/{department}")]
        public IActionResult Preview(string department)
        {
            try
            {
                string sharedFolderPath = @"\\server\shared\documents";
                string documentPath = Path.Combine(sharedFolderPath, $"{department}.pdf");

                //if (!System.IO.File.Exists(documentPath))
                //{
                //    return NotFound("PDF dokument nenalezen.");
                //}
                var fileUrl = $"https://dagrs.berkeley.edu/sites/default/files/2020-01/sample.pdf"; // example pdf
                return Ok(fileUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("print/{department}")]
        public IActionResult Print(string department)
        {
            try
            {
                return Ok(); // for testing success modal 
                string sharedFolderPath = @"\\server\shared\documents";
                string documentPath = Path.Combine(sharedFolderPath, $"{department}.pdf");

                if (!System.IO.File.Exists(documentPath))
                {
                    return NotFound("PDF dokument nenalezen.");
                }
                PrintDocument(documentPath, department);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private void PrintDocument(string documentPath, string department)
        {
            if (!_printerAddresses.TryGetValue(department, out var printerAddress))
            {
                throw new ArgumentException("Neplatné oddělení");
            }
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"Start-Process -FilePath '{documentPath}' -ArgumentList '/t \"{printerAddress}\"' -NoNewWindow\"",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process process = new Process
            {
                StartInfo = info
            };

            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}