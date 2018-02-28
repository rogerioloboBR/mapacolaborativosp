using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using smul.mapa.final.core.Data;
using smul.mapa.final.core.Models;

namespace smul.mapa.final.core.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ApplicationDbContext _context;

        public RegistrationsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        // GET: Registrations
        public async Task<IActionResult> Index(string searchString)
        {
            var registrations = from reg in _context.Registration select reg;
            if(!String.IsNullOrEmpty(searchString))
            {
                registrations = registrations.Where(s => s.Segmento.Contains(searchString));
            }
            return View(await registrations.ToListAsync());
        }

       
        // GET: Registrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration
                .SingleOrDefaultAsync(m => m.ID == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // GET: Registrations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Registrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,NomeInstituicao,TelefoneFixo,TelefoneCelular,Email,Site,ProfileFacebook,NomeRepresentante,Rua,Numero,CEP,PrefeituraRegional,Segmento,Tematica,TempoDeAtucao,Registro,Lat,Lng")] Registration registration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(registration);
        }

        // GET: Registrations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration.SingleOrDefaultAsync(m => m.ID == id);
            if (registration == null)
            {
                return NotFound();
            }
            return View(registration);
        }

        // POST: Registrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,NomeInstituicao,TelefoneFixo,TelefoneCelular,Email,Site,ProfileFacebook,NomeRepresentante,Rua,Numero,CEP,PrefeituraRegional,Segmento,Tematica,TempoDeAtucao,Registro,Lat,Lng")] Registration registration)
        {
            if (id != registration.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationExists(registration.ID))
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
            return View(registration);
        }

        // GET: Registrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration
                .SingleOrDefaultAsync(m => m.ID == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: Registrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.Registration.SingleOrDefaultAsync(m => m.ID == id);
            _context.Registration.Remove(registration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationExists(int id)
        {
            return _context.Registration.Any(e => e.ID == id);
        }

        public async Task<IActionResult> OnPostExport()
        {

            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"Atores_Urbanos.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                var resgistrations = _context.Registration.ToList();
                resgistrations.Select(x => new {
                    x.ID,
                    x.NomeInstituicao,
                    x.TelefoneFixo,
                    x.TelefoneCelular,
                    x.NomeRepresentante,
                    x.Segmento,
                    x.Tematica,
                    x.Email
                });
                //adiciona uma nova planilha a pasta de trabalho vazia
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Atores_Urbanos");
                //Primeiro o cabeçalho
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Nome da Instituição";
                worksheet.Cells[1, 3].Value = "Telefone Fixo";
                worksheet.Cells[1, 4].Value = "Telefone Celular";
                worksheet.Cells[1, 5].Value = "Nome do Representante";
                worksheet.Cells[1, 6].Value = "E-mail";
                var i = 2;
                foreach (var item in resgistrations)
                {


                    worksheet.Cells["A" + i].Value = item.ID;
                    worksheet.Cells["B" + i].Value = item.NomeInstituicao;
                    worksheet.Cells["C" + i].Value = item.TelefoneFixo;
                    worksheet.Cells["D" + i].Value = item.TelefoneCelular;
                    worksheet.Cells["E" + i].Value = item.NomeRepresentante;
                    worksheet.Cells["F" + i].Value = item.Email;
                    i++;
                }




                package.Save();

            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);




        }
    }
}
