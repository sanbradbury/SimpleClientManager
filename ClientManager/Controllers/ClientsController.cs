using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClientManager.Models;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
//using System.Net.Mail;
using OfficeOpenXml;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net;
using MailKit.Security;

namespace ClientManager.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ClientManagementContext _context;

        public ClientsController(ClientManagementContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        
        public IActionResult Create()
        {
            return View();
        }

       



        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,ClientName,ClientSurname,ContactNumber,CallCenterName,Email,Idnumber,AccountBalance,CaptureDate,CapturedBy,PaymentsToDate")] Client client)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(client.ClientName) ||
                    string.IsNullOrWhiteSpace(client.ClientSurname) ||
                    string.IsNullOrWhiteSpace(client.ContactNumber) ||
                    string.IsNullOrWhiteSpace(client.CallCenterName) ||
                    string.IsNullOrWhiteSpace(client.Email) ||
                    string.IsNullOrWhiteSpace(client.Idnumber) ||
                    client.AccountBalance == null ||
                    client.CaptureDate == null ||
                    string.IsNullOrWhiteSpace(client.CapturedBy))
                {
                    TempData["errorMessage"] = "All fields are required.";
                    return View(client); // Pass the client model back to the view
                }

                //Use regex to check that the email is in correct format
                if (!Regex.IsMatch(client.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                {
                    TempData["errorMessage"] = "Invalid email format.";
                    return View(client); // Pass the client model back to the view
                }

                // Check if name and surname fields do not exceed 50 characters
                if (client.ClientName.Length > 50 || client.ClientSurname.Length > 50)
                {
                    TempData["errorMessage"] = "Name and surname fields should not contain more than 50 characters respectively.";
                    return View(client); // Pass the client model back to the view
                }

                if (client.Idnumber.Length != 13)
                {
                    TempData["errorMessage"] = "ID number should contain exactly 13 digits.";
                    return View(client); // Pass the client model back to the view
                }

                // Preprocess contact number to include +27 extension
                if (!client.ContactNumber.StartsWith("+27"))
                {
                    client.ContactNumber = "+27" + client.ContactNumber.TrimStart('0');
                }

                using (var context = new ClientManagementContext())
                {
                    if (ModelState.IsValid)
                    {
                        context.Add(client);
                        await context.SaveChangesAsync();
                        TempData["successMessage"] = "Client added successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    return View(client);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(client); // Pass the client model back to the view
            }
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,ClientName,ClientSurname,ContactNumber,CallCenterName,Email,Idnumber,AccountBalance,CaptureDate,CapturedBy,PaymentsToDate")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }

        public async Task<IActionResult> ExportToExcel(string emailAddress)
        {
            try
            {
                // Set license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

                var clients = await _context.Clients.ToListAsync(); // Retrieve all clients from the database

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    // Create a new worksheet named "Clients"
                    var worksheet = package.Workbook.Worksheets.Add("Clients");

                    // Add header row
                    worksheet.Cells["A1"].LoadFromCollection(clients, true);

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
                builder.Attachments.Add("Clients.xlsx", ms);
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }



    }
}
