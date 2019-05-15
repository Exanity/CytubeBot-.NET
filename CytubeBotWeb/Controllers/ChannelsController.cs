using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CytubeBotWeb.Data;
using CytubeBotWeb.Models;
using CytubeBotCore;

namespace CytubeBotWeb.Controllers
{
    public class ChannelsController : Controller
    {
        private readonly ServerDbContext _context;
        public List<CytubeBot> _runningBots;

        public ChannelsController(ServerDbContext context, List<CytubeBot> runningBots)
        {
            _context = context;
            _runningBots = runningBots;
        }

        // GET: Channels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Channels.ToListAsync());
        }

        // GET: Channels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelModel = await _context.Channels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (channelModel == null)
            {
                return NotFound();
            }

            return View(channelModel);
        }

        // GET: Channels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Channels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChannelName,ServerModelId")] ChannelModel channel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(channel);
                var server = _context.Servers.Find(channel.ServerModelId);
                server.Channels.Add(channel);
                _context.Update(server);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit","Servers", new { Id = channel.ServerModelId });
            }
            return View(channel);
        }

        // GET: Channels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelModel = await _context.Channels.FindAsync(id);
            if (channelModel == null)
            {
                return NotFound();
            }
            return View(channelModel);
        }

        // POST: Channels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChannelName")] ChannelModel channel)
        {
            if (id != channel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(channel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChannelModelExists(channel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(channel);
        }

        // GET: Channels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelModel = await _context.Channels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (channelModel == null)
            {
                return NotFound();
            }

            return View(channelModel);
        }

        // POST: Channels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var channelModel = await _context.Channels.FindAsync(id);
            _context.Entry(channelModel).Reference(channel => channel.Server).Load();
            var test = channelModel.Server;
            var serverId = channelModel.ServerModelId;
            var bot = _runningBots.Find(x => (x.Channel == channelModel.ChannelName) && (x.Username == channelModel.Server.Username) && (x.Server == channelModel.Server.Host));
            if (bot != null)
            {
                bot.Disconnect();
                _runningBots.Remove(bot);
                bot = null;
            }
            _context.Channels.Remove(channelModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Servers", new { Id = serverId });
        }

        private bool ChannelModelExists(int id)
        {
            return _context.Channels.Any(e => e.Id == id);
        }
    }
}
