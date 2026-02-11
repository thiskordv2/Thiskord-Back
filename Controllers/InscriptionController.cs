using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Services;
using System.Text.Json;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class   InscriptionController : Controller
    {
        private readonly InscriptionService _inscriptionService;
        private readonly LogService _logService;

        public InscriptionController(InscriptionService inscriptionService, LogService logService)
        {
            _inscriptionService = inscriptionService;
            _logService = logService;
        }


                }
            }