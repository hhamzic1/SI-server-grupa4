using Azure.Storage.Blobs;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.License;
using MonitorWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MonitorWebAPI.Helpers
{
    public class CreatePDF
    {
        private readonly Services.IBlobService _blobService;

        public CreatePDF(Services.IBlobService blobService)
        {
            _blobService = blobService;
        }

        public static Document createPDF(int reportId)
        {

            monitorContext mc = new monitorContext();

            Report report = mc.Reports
                .Where(r => r.ReportId == reportId)
                .FirstOrDefault();

            string instanceName = report.Name + "-" + report.NextDate.ToString() + ".pdf";
            string destination = "../../data/"+instanceName;
            LicenseKey.LoadLicenseFile("Helpers\\itextkey.xml");

            DateTime startDate = HelperMethods.GetStartDate(report.NextDate, report.Frequency);

            PdfWriter pdfWriter = new PdfWriter(destination);
            PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            pdfDocument.AddNewPage();
            Document document = new Document(pdfDocument);

            Paragraph header = new Paragraph("Monitor Reporting")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(96, 116, 148))
                .SetBold()
                .SetFontSize(25);

            Paragraph ReportName = new Paragraph("Report name: " + report.Name +
                "\n" + report.Frequency + " report: " +
                startDate.ToString("G", CultureInfo.CreateSpecificCulture("de-DE")) + " - " +
                report.NextDate.ToString("G", CultureInfo.CreateSpecificCulture("de-DE")))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(16)
                .SetBorderBottom(new SolidBorder(new DeviceRgb(189, 189, 189), 1))
                .SetFontColor(new DeviceRgb(189, 189, 189))
                .SetMarginTop(10)
                .SetMarginBottom(25);

            //popraviti da se sa fronta salje ispravan query i da se to promijeni u bazi 

            QueryModel queryModel = System.Text.Json.JsonSerializer.Deserialize<QueryModel>(report.Query);
            queryModel.GenerateQuery();


            int groupId = queryModel.group;

            Group selectedGroup = mc.Groups.Where(g => g.GroupId == groupId).FirstOrDefault();

            //svi devices za izabranu grupu
            List<Device> devices = mc.Devices.Join(mc.DeviceGroups,
                p => p.DeviceId,
                q => q.DeviceId,
                (p, q) => new { Device = p, DeviceGroup = q })
                .Where(d => d.DeviceGroup.GroupId == groupId) //dodati datume
                .Select(dd => new Device
                {
                    DeviceId = dd.Device.DeviceId,
                    Name = dd.Device.Name,
                    Location = dd.Device.Location,
                    LocationLatitude = dd.Device.LocationLatitude,
                    LocationLongitude = dd.Device.LocationLongitude,
                    Status = dd.Device.Status,
                    LastTimeOnline = dd.Device.LastTimeOnline,
                    InstallationCode = dd.Device.InstallationCode,
                    DeviceUid = dd.Device.DeviceUid
                }).ToList();

            List<Device> allDevices = new List<Device>();

            foreach (var device in devices)
            {
                if (queryModel.query.Eval(device))
                {
                    allDevices.Add(device);
                }
            }


            List<AllInfoForDevice> allInfoForDevices = new List<AllInfoForDevice>();

            foreach (var device in allDevices)
            {
                Averages averageInfo = new Averages() { };
                averageInfo = mc.DeviceStatusLogs
                    .Where(d => d.DeviceId == device.DeviceId)
                    .GroupBy(d => d.DeviceId)
                    .Select(d => new Averages
                    {
                        CpuUsage = (double)d.Average(d => d.CpuUsage),
                        RamUsage = (double)d.Average(d => d.RamUsage),
                        Hddusage = (double)d.Average(d => d.Hddusage),
                        Gpuusage = (double)d.Average(d => d.Gpuusage)
                    }).FirstOrDefault();

                AllInfoForDevice allInfo = new AllInfoForDevice
                {
                    Device = device,
                    Averages = averageInfo
                };

                allInfoForDevices.Add(allInfo);

            }


            int numberOfSelectedCols = queryModel.select.Count();

            Table table = new Table(numberOfSelectedCols, true);

            List<Cell> labels = new List<Cell>();

            List<String> stringLabels = new List<string>();

            foreach (var label in queryModel.select)
            {
                switch (label)
                {
                    case "name": stringLabels.Add("Device Name"); break;
                    case "avgRamUsage": stringLabels.Add("Average RAM Usage"); break;
                    case "avgGpuUsage": stringLabels.Add("Average GPU Usage"); break;
                    case "quarterlyCpuUsage": stringLabels.Add("Quarterly CPU Usage"); break;
                    case "diskUtilization": stringLabels.Add("Disk Utilization"); break;
                    case "location": stringLabels.Add("Location"); break;
                    case "latitude": stringLabels.Add("Latitude"); break;
                    case "longitude": stringLabels.Add("Longitude"); break;
                    case "status": stringLabels.Add("Status"); break;
                    case "groupName": stringLabels.Add("Group Name"); break;
                }
            }


            for (int i = 0; i < numberOfSelectedCols; i++)
            {
                table.AddCell(new Cell().Add(new Paragraph(stringLabels[i])
                .SetFontColor(new DeviceRgb(96, 116, 148))
                .SetBold()
                .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE)));
            }


            foreach (var allInfoForDevice in allInfoForDevices)
            {
                var name = new Paragraph(allInfoForDevice.Device.Name);
                var location = new Paragraph(allInfoForDevice.Device.Location);
                var latitude = new Paragraph(allInfoForDevice.Device.LocationLatitude.ToString());
                var longitude = new Paragraph(allInfoForDevice.Device.LocationLongitude.ToString());
                var status = new Paragraph(allInfoForDevice.Device.Status.ToString());
                var lastTimeOnline = new Paragraph(allInfoForDevice.Device.LastTimeOnline.ToString());
                Paragraph ram, gpu, cpu, hdd;
                if (allInfoForDevice.Averages == null)
                {
                    ram = new Paragraph("Nema podataka o RamUsage");
                    gpu = new Paragraph("Nema podataka o GpuUsage");
                    cpu = new Paragraph("Nema podataka o CpuUsage");
                    hdd = new Paragraph("Nema podataka o HddUsage");
                }
                else
                {
                    ram = new Paragraph(allInfoForDevice.Averages.RamUsage.ToString());
                    gpu = new Paragraph(allInfoForDevice.Averages.Gpuusage.ToString());
                    cpu = new Paragraph(allInfoForDevice.Averages.CpuUsage.ToString());
                    hdd = new Paragraph(allInfoForDevice.Averages.Hddusage.ToString());
                }

                var groupName = new Paragraph(selectedGroup.Name);

                List<Cell> tableDataCells = new List<Cell>();

                foreach (var label in queryModel.select)
                {
                    switch (label)
                    {
                        case "name": tableDataCells.Add(new Cell().Add(name)); break;
                        case "avgRamUsage": tableDataCells.Add(new Cell().Add(ram)); break;
                        case "avgGpuUsage": tableDataCells.Add(new Cell().Add(gpu)); break;
                        case "quarterlyCpuUsage": tableDataCells.Add(new Cell().Add(cpu)); break;
                        case "diskUtilization": tableDataCells.Add(new Cell().Add(hdd)); break;
                        case "location": tableDataCells.Add(new Cell().Add(location)); break;
                        case "latitude": tableDataCells.Add(new Cell().Add(latitude)); break;
                        case "longitude": tableDataCells.Add(new Cell().Add(longitude)); break;
                        case "status": tableDataCells.Add(new Cell().Add(status)); break;
                        case "groupName": tableDataCells.Add(new Cell().Add(groupName)); break;
                    }
                }

                foreach (var cell in tableDataCells)
                {
                    table.AddCell(cell);
                }

            }

            document.Add(header);
            document.Add(ReportName);
            document.Add(table);
            document.Close();

            return document;
        }
    }
}
