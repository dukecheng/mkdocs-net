using AgileLabs;
using Niusys.Docs.Core.Projects;
using Hangfire;
using Hangfire.Server;
using Hangfire.Console;
using Niusys.Docs.Core.ProjectHttpClients;
using Niusys.Docs.Core.DataStores;

namespace Niusys.Docs.Core
{
    public class DocProjectService : IScoped
    {
        private readonly MkdocsDatabase _mkdocsDatabase;
        private readonly DocProjectHttpClientFactory _projectHttpClientFactory;
        private readonly IWorkContext _workContext;

        public DocProjectService(MkdocsDatabase mkdocsDatabase, DocProjectHttpClientFactory projectHttpClientFactory, IWorkContext workContext)
        {
            _mkdocsDatabase = mkdocsDatabase;
            _projectHttpClientFactory = projectHttpClientFactory;
            _workContext = workContext;
        }
        [AutomaticRetry(Attempts = 1)]
        public async Task BranchListSyncTrigger(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var projectList = _mkdocsDatabase.Search<DocProject>();
            context?.WriteLine($"Total found {projectList.Count()} projects, workcontext #{_workContext.ContextId}");
            foreach (var project in projectList)
            {
                BackgroundJob.Enqueue<DocProjectService>((service) => service.SyncBranch(project.Id, 1, null, default));
                BackgroundJob.Enqueue<DocProjectService>((service) => service.SyncTag(project.Id, 2, null, default));
            }
            await Task.CompletedTask;
        }

        [AutomaticRetry(Attempts = 1)]
        public async Task SyncBranch(long projectId, int type, PerformContext context = null, CancellationToken cancellationToken = default)
        {
            context?.WriteLine($"SyncBranch-{type}-workcontext #{_workContext.ContextId}");
            var project = _mkdocsDatabase.Get<DocProject>(projectId);
            var currentProjectList = _mkdocsDatabase.Search<DocProjectBranch>(x => x.ProjectId == project.Id).ToList();
            var projectRemoteClient = _projectHttpClientFactory.CreateHttpClient(project.HostType, _workContext);

            var alreadyExistNameList = currentProjectList.Select(x => x.Name).ToList();
            var remoteBranchNameList = new List<string>();

            int maxSearchTimes = 10;
            for (int i = 1; i <= maxSearchTimes; i++)
            {
                var remoteNameList = await projectRemoteClient.GetBranchListAsync(project, i, 100);

                if (remoteNameList == null || !remoteNameList.Any())
                {
                    // 如果没有了直接返回
                    break;
                }
                remoteBranchNameList.AddRange(remoteNameList);
            }

            var needAddBranchNameList = remoteBranchNameList.Except(alreadyExistNameList).ToList();
            var needRemoveBranchNameList = alreadyExistNameList.Except(remoteBranchNameList).ToList();

            _mkdocsDatabase.Delete<DocProjectBranch>(x => x.ProjectId == project.Id && needRemoveBranchNameList.Contains(x.Name));

            var worker = new Snowflake.Core.IdWorker(1, 1);
            _mkdocsDatabase.Insert<DocProjectBranch>(needAddBranchNameList.Select(x => new DocProjectBranch
            {
                Id = worker.NextId(),
                ProjectId = project.Id,
                Name = x,
                CreatedTime = DateTime.Now
            }.SetProperty(x)).ToList());
            await Task.CompletedTask;
        }

        [AutomaticRetry(Attempts = 1)]
        public async Task SyncTag(long projectId, int type, PerformContext context = null, CancellationToken cancellationToken = default)
        {
            context?.WriteLine($"SyncTag-{type}-workcontext #{_workContext.ContextId}");
            var project = _mkdocsDatabase.Get<DocProject>(projectId);
            var currentProjectList = _mkdocsDatabase.Search<DocProjectTag>(x => x.ProjectId == project.Id).ToList();
            var projectRemoteClient = _projectHttpClientFactory.CreateHttpClient(project.HostType, _workContext);

            var alreadyExistNameList = currentProjectList.Select(x => x.Name).ToList();
            var remoteBranchNameList = new List<string>();

            int maxSearchTimes = 10;
            for (int i = 1; i <= maxSearchTimes; i++)
            {
                var remoteNameList = await projectRemoteClient.GetTagListAsync(project, i, 100);

                if (remoteNameList == null || !remoteNameList.Any())
                {
                    // 如果没有了直接返回
                    break;
                }
                remoteBranchNameList.AddRange(remoteNameList);
            }

            var needAddBranchNameList = remoteBranchNameList.Except(alreadyExistNameList).ToList();
            var needRemoveBranchNameList = alreadyExistNameList.Except(remoteBranchNameList).ToList();

            _mkdocsDatabase.Delete<DocProjectTag>(x => x.ProjectId == project.Id && needRemoveBranchNameList.Contains(x.Name));

            var worker = new Snowflake.Core.IdWorker(1, 1);
            _mkdocsDatabase.Insert<DocProjectTag>(needAddBranchNameList.Select(x => new DocProjectTag
            {
                Id = worker.NextId(),
                ProjectId = project.Id,
                Name = x,
                CreatedTime = DateTime.Now
            }.SetVersions(x)).ToList());
            await Task.CompletedTask;
        }
    }
}
