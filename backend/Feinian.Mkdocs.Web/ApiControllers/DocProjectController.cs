using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Niusys.Docs.Core.DataStores;
using Niusys.Docs.Core.Projects;

namespace Niusys.Docs.Web.ApiControllers;

[Route("api/doc_project")]
[ApiController]
public class DocProjectController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly MkdocsDatabase _mkdocsDatabase;

    public DocProjectController(IMapper mapper, MkdocsDatabase mkdocsDatabase)
    {
        _mapper = mapper;
        _mkdocsDatabase = mkdocsDatabase;
    }

    [HttpPost, Route("search")]
    public List<DocProject> SearchProject()
    {
        return _mkdocsDatabase.Search<DocProject>().ToList();
    }

    [HttpGet, Route("get")]
    public DocProject Get(long projectId)
    {
        return _mkdocsDatabase.Get<DocProject>(projectId);
    }

    [HttpPost, Route("add")]
    public DocProject AddProject([FromBody] DocProject project)
    {
        var worker = new Snowflake.Core.IdWorker(1, 1);
        project.Id = worker.NextId();
        _mkdocsDatabase.Insert(project);
        return project;
    }

    [HttpPost, Route("update")]
    public DocProject UpdateProject([FromBody] DocProject project)
    {
        _mkdocsDatabase.Update(project);
        return project;
    }

    [HttpPost, Route("delete")]
    public void DeleteProject(long projectId)
    {
        _mkdocsDatabase.Delete<DocProject>(projectId);
    }
}
