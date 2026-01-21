using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Services;
using Thiskord_Back.Models.Project;
using System.Text.Json;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly LogService _logService;

        public ProjectController(ProjectService projectService, LogService logService)
        {
            _projectService = projectService;
            _logService = logService;
        }

        // GET: HomeController/Create
        //public actionresult create()
        //{
        //    return view();
        //}

        // POST: HomeController/Create
        [HttpPost("create")]
        public ActionResult Create([FromBody] Project.ProjectRequest req)
        {
            if (string.IsNullOrEmpty(req.name))
            {
                _logService.CreateLog("Il manque le titre");
                return BadRequest(new { error = "Il n'y a pas de titre pour le projet" });
            }

            try
            {
                // envois des données au service de création de projet
                int pro = _projectService.CreateProject(req.name, req.description);

                return Ok(pro);

            }
            catch (Exception ex)
            {

                // TODO: ajouter un message d'erreur au logs

                return BadRequest(ex.Message);

            }
        }

        [HttpGet("all")]
        public ActionResult GetAllProjects()
        {
            try
            {
                var pro = _projectService.GetAllProjects();
                return Ok(pro);

            }
            catch (Exception ex)
            {
                _logService.CreateLog($"Erreur récupération projets : {ex.Message}");
                return StatusCode(500, "Une erreur interne est survenue lors de la récupération des projets.");
            }


        }
    }
}