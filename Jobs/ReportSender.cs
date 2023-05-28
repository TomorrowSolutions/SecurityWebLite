using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Quartz;
using SecurityLite.Models;
using System;
using System.Net;
using System.Net.Mail;

namespace SecurityLite.Jobs
{
    public class ReportSender : IJob
    {
        string file_path_template;
        string file_path_report;
        private readonly ModelsContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public ReportSender(ModelsContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public void PrepareReport()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(file_path_template))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Охранная организация";
                excelPackage.Workbook.Properties.Title = "Список заказов";
                excelPackage.Workbook.Properties.Subject = "Заказы";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                excelPackage.Workbook.Worksheets["Заказы"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Order> orders = _context.Orders.Include(o => o.Client).Include(o => o.Manager).ToList();

                foreach (Order o in orders)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = o.Id;
                    worksheet.Cells[startLine, 3].Value = o.Manager.GetFIO;
                    worksheet.Cells[startLine, 4].Value = o.Client.GetFIO;
                    worksheet.Cells[startLine, 5].Value = o.DateOfSigning.ToString();
                    worksheet.Cells[startLine, 6].Value = o.DateOfComplete == null ? "---" : o.DateOfComplete.ToString();
                    worksheet.Cells[startLine, 7].Value = o.price == null ? "---" : o.price;
                    startLine++;
                }
                startLine = 3;
                ExcelWorksheet worksheet2 =
                excelPackage.Workbook.Worksheets["Подробности"];
                List<OrderDetail> orderDetails = _context.OrderDetails.Include(o => o.GuardedObject).Include(o => o.Order).Include(o => o.Service).ToList();
                foreach (OrderDetail od in orderDetails)
                {
                    worksheet2.Cells[startLine, 1].Value = startLine - 2;
                    worksheet2.Cells[startLine, 2].Value = od.Id;
                    worksheet2.Cells[startLine, 3].Value = od.Order.Id;
                    worksheet2.Cells[startLine, 4].Value = od.Service.Name;
                    worksheet2.Cells[startLine, 5].Value = od.GuardedObject.Name;
                    worksheet2.Cells[startLine, 6].Value = od.Quantity;
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(file_path_report);
            }
        }
        public async Task Execute(IJobExecutionContext context)
        {
            file_path_template = _appEnvironment.WebRootPath + "/Reports/OrderReportTemplate.xlsx";
            file_path_report = _appEnvironment.WebRootPath + "/Reports/OrderReport.xlsx";
            try
            {
                if (File.Exists(file_path_report)) File.Delete(file_path_report);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            PrepareReport();
            if (File.Exists(file_path_report))
                await Console.Out.WriteLineAsync("smth");
            MailAddress from = new MailAddress("ZenScripts@yandex.ru", "mailfrom");
            MailAddress to = new MailAddress("vdm_g@mail.ru");
            MailMessage m = new MailMessage(from,to);
            m.Subject = "Отчет по заказам";
            m.Body = "<h2>Здравствуйте, к этому письму приложен файл с отчетом</h2>";
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);            
            smtp.Credentials = new NetworkCredential("ZenScripts@yandex.ru", "elsauzwwxhvempca");
            smtp.EnableSsl = true;
            m.Attachments.Add(new Attachment(file_path_report));
            await smtp.SendMailAsync(m);
            m.Dispose();
        }
    }
}
