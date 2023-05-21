// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Niusys.Docs.Core.DataStores;
using Niusys.Docs.Core.Projects;

namespace Niusys.Docs.Web.Components;

[ViewComponent(Name = "MarkdownHeader")]
public class MarkdownHeaderViewComponent : ViewComponent
{
    private readonly MkdocsDatabase _docRepositoryManager;

    public MarkdownHeaderViewComponent(MkdocsDatabase docRepositoryManager)
    {
        _docRepositoryManager = docRepositoryManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var repos = _docRepositoryManager.Search<DocProject>().ToList();
        await Task.CompletedTask;
        return View(repos);
    }
}
