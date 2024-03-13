using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClientManager.Models;
using System.Runtime.Intrinsics.X86;
using MailKit.Security;
using MimeKit;
using OfficeOpenXml;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using System.Net;


namespace ClientManager.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ClientManagementContext _context;

        public PaymentsController(ClientManagementContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var clientManagementContext = _context.Payments.Include(p => p.Client);
            return View(await clientManagementContext.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Client)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,ClientId,DateOfPayment,AmountOfPayment,ReferenceForPayment")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                var client = await _context.Clients.FindAsync(payment.ClientId);
                //Update client table
                client.PaymentsToDate += payment.AmountOfPayment;
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId", payment.ClientId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId", payment.ClientId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,ClientId,DateOfPayment,AmountOfPayment,ReferenceForPayment")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId", payment.ClientId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Client)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }

        public async Task<IActionResult> ExportToExcel(string emailAddress)
        {
            try
            {
                // Set license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var payments = await _context.Payments.ToListAsync(); // Retrieve all clients from the database

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    // Create a new worksheet named "Payments"
                    var worksheet = package.Workbook.Worksheets.Add("Payments");

                    // Add header row
                    worksheet.Cells["A1"].LoadFromCollection(payments, true);

                    // Save Excel data to a memory stream
                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        stream.Position = 0;

                        // Send email with the Excel file as an attachment
                        await SendEmailWithAttachment(emailAddress, stream.ToArray());
                    }
                }

                TempData["successMessage"] = "Excel file sent to email successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Method to send email with attachment
        private async Task SendEmailWithAttachment(string emailAddress, byte[] attachmentData)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Santana", "bradburysantana@gmail.com"));
            message.To.Add(new MailboxAddress("", emailAddress));
            message.Subject = "Client Data Excel File";

            var builder = new BodyBuilder();
            builder.TextBody = "Please find attached the client data Excel file.";

            using (var ms = new MemoryStream(attachmentData))
            {
                builder.Attachments.Add("Payments.xlsx", ms);
                message.Body = builder.ToMessageBody();
            }

            using (var client = new SmtpClient())
            {
                // configuring SMTP settings
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("bradburysantana@gmail.com", "jjcv setv wujf idqd");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}