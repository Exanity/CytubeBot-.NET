using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CytubeBotWeb.Data;
using CytubeBotWeb.Models;
using CytubeBotWeb.ViewModels;
using CytubeBotCore;

namespace CytubeBotWeb.Controllers
{
    public class ServersController : Controller
    {
        private readonly ServerDbContext _context;
        public List<CytubeBot> _runningBots;

        public ServersController(ServerDbContext context, List<CytubeBot> runningBots)
        {
            _context = context;
            _runningBots = runningBots;
        }

        // GET: Server
        public async Task<IActionResult> Index()
        {
            ServerIndexViewModel model = new ServerIndexViewModel()
            {
                Servers = await _context.Servers.Include(server => server.Channels).ToListAsync(),
                RunningBots = _runningBots
            };

            return View(model);
        }

        // GET: Server/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serverModel = await _context.Servers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serverModel == null)
            {
                return NotFound();
            }

            return View(serverModel);
        }

        // GET: Server/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Server/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Host,Username,Password")] ServerModel Server)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Server);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Server);
        }

        // GET: Server/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var serverChannelModel = new ServerChannelViewModel();

            var serverModel = await _context.Servers.FindAsync(id);
            if (serverModel == null)
            {
                return NotFound();
            }
            else
            {
                serverChannelModel.Server = serverModel;
                var c = await _context.Channels.Where(channel => channel.ServerModelId == id).ToListAsync();
                if(c != null)
                {
                    serverChannelModel.Channels = c;
                }
            }


            return View(serverChannelModel);
        }

        // POST: Server/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Host,Username,Password")] ServerModel Server)
        {
            if (id != Server.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Server);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServerModelExists(Server.Id))
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
            return View(Server);
        }

        // GET: Server/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serverModel = await _context.Servers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serverModel == null)
            {
                return NotFound();
            }

            return View(serverModel);
        }

        // POST: Server/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serverModel = await _context.Servers.FindAsync(id);
            _context.Servers.Remove(serverModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> StartServerBot(int id)
        {
            var serverModel = await _context.Servers.FindAsync(id);
            var channels = await _context.Channels.Where(channel => channel.ServerModelId == id).ToListAsync();
            if (channels != null)
            {
                foreach (var channel in channels)
                {
                    var bot = _runningBots.Find(x => (x.Channel == channel.ChannelName) && (x.Username == serverModel.Username) && (x.Server == serverModel.Host));
                    if (bot == null)
                    {
                        CytubeBot cytubebot = new CytubeBot(serverModel.Host, channel.ChannelName, serverModel.Username, serverModel.Password);
                        cytubebot.Connect();
                        _runningBots.Add(cytubebot);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StopServerBot(int id)
        {
            var serverModel = await _context.Servers.FindAsync(id);
            var channels = await _context.Channels.Where(channel => channel.ServerModelId == id).ToListAsync();
            if (channels != null)
            {
                foreach (var channel in channels)
                {
                    var bot = _runningBots.Find(x => (x.Channel == channel.ChannelName) && (x.Username == serverModel.Username) && (x.Server == serverModel.Host));
                    if (bot != null)
                    {
                        bot.Disconnect();
                        _runningBots.Remove(bot);
                        bot = null;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServerModelExists(int id)
        {
            return _context.Servers.Any(e => e.Id == id);
        }
    }
}
