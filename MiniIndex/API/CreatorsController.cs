﻿using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Core.Submissions;
using MiniIndex.Minis;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/creators")]
    public class CreatorsController : Controller
    {
        public CreatorsController(
                MiniIndexContext context,
                IMapper mapper,
                IMediator mediator,
                IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _apiKey = configuration["AutoCreateKey"];
        }

        private readonly MiniIndexContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly string _apiKey;

        [EnableCors("SpecificOrigins")]
        [HttpGet("mini")]
        public async Task<IActionResult> GetMini(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Mini mini = await _context.Mini.FirstOrDefaultAsync(m=>m.ID == id);

            if (mini == null)
            {
                return NotFound();
            }

            return Ok(mini);
        }

        [EnableCors("SpecificOrigins")]
        [HttpGet("browse")]
        public async Task<IActionResult> SearchMinis(
            [FromQuery]int pageSize = 21,
            [FromQuery]int pageIndex = 1)
        {
            //Mild hack - There's some case where pageIndex is hitting 0 and I can't tell how/why. (GitHub #182)
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            List<Creator> countQuery = await _context.Set<Mini>()
                .Include(m => m.Creator).ThenInclude(c => c.Sites)
                .Select(m => m.Creator)
                .ToListAsync();

            Dictionary<Creator, int> CreatorCounts = countQuery
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .ToDictionary(k => k.Key, v => v.Count());

            return Ok(CreatorCounts.Select(k=>new
                { 
                    ID = k.Key.ID,
                    Name = k.Key.Name,
                    MiniCount = k.Value,
                    SourceSites = k.Key.Sites.Select(ss=> new
                    {
                        SiteName = ss.SiteName,
                        URL = ss.CreatorPageUri
                    })
                }
            ));
        }
    }
}