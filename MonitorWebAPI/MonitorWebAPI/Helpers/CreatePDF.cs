using Azure.Storage.Blobs;
using MonitorWebAPI.Models;
using SautinSoft.Document;
using SautinSoft.Document.Tables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MonitorWebAPI.Helpers
{
    public class CreatePDF
    {
        public static string GenerateInstanceName(int reportId)
        {
            monitorContext mc = new monitorContext();

            Report report = mc.Reports
                .Where(r => r.ReportId == reportId)
                .FirstOrDefault();
            Random random = new Random();
            string instanceName = "Report-" + random.Next(9999999) + ".pdf";
            return instanceName;
        }

        public static DocumentCore createPDF(int reportId, string instanceName)
        {
            monitorContext mc = new monitorContext();

            Report report = mc.Reports
                .Where(r => r.ReportId == reportId)
                .FirstOrDefault();

            string documentPath = "/root/SI-server-grupa4/MonitorWebAPI/MonitorWebAPI/data/" + instanceName;
            DocumentCore dc = new DocumentCore();
            Section s = new Section(dc);
            dc.Sections.Add(s);


            DateTime startDate = HelperMethods.GetStartDate(report.NextDate, report.Frequency);

            Paragraph header = new Paragraph(dc);
            header.ParagraphFormat.Alignment = HorizontalAlignment.Center;
            header.Content.Start.Insert("Monitor Reporting", new CharacterFormat() { FontColor = new Color("#607494"), Size = 20.0, Bold = true });

            Paragraph reportName = new Paragraph(dc);
            reportName.ParagraphFormat.Alignment = HorizontalAlignment.Left;
            reportName.Content.Start.Insert("Report name: " + report.Name,
                new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 12.0, Bold = true });

            Paragraph date = new Paragraph(dc);
            date.ParagraphFormat.Alignment = HorizontalAlignment.Left;
            date.Content.Start.Insert(report.Frequency + " report: " +
                startDate.ToString("G", CultureInfo.CreateSpecificCulture("de-DE")) + " - " +
                report.NextDate.ToString("G", CultureInfo.CreateSpecificCulture("de-DE")),
                new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

            s.Blocks.Add(header);
            s.Blocks.Add(reportName);
            s.Blocks.Add(date);

            // ---------------------------------------------------------------------------
            // ---------------------------------------------------------------------------

            QueryModel queryModel = System.Text.Json.JsonSerializer.Deserialize<QueryModel>(report.Query);
            queryModel.GenerateQuery();


            int groupId = queryModel.group;

            Group selectedGroup = mc.Groups.Where(g => g.GroupId == groupId).FirstOrDefault();

            //svi devices za izabranu grupu
            List<Device> devices = mc.Devices.Join(mc.DeviceGroups,
                p => p.DeviceId,
                q => q.DeviceId,
                (p, q) => new { Device = p, DeviceGroup = q })
                .Where(d => d.DeviceGroup.GroupId == groupId)
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

            /* Old way
            foreach (var device in devices)
            {
                if (true || queryModel.query.Eval(device))
                {
                    allDevices.Add(device);
                }
            }
            */
            allDevices = devices;


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

            List<AllInfoForDevice> allInfoForDevicesFilter = new List<AllInfoForDevice>();
            foreach (var device in allInfoForDevices)
            {
                if (queryModel.query.Eval(device))
                {
                    allInfoForDevicesFilter.Add(device);
                }
            }
            allInfoForDevices = allInfoForDevicesFilter;


            int numberOfSelectedCols = queryModel.select.Count();

            Table table = new Table(dc);
            double width = LengthUnitConverter.Convert(200, LengthUnit.Millimeter, LengthUnit.Point);
            table.TableFormat.PreferredWidth = new TableWidth(width, TableWidthUnit.Point);
            table.TableFormat.Alignment = HorizontalAlignment.Left;

           

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

            TableRow firstRow = new TableRow(dc);
            for (int i = 0; i < numberOfSelectedCols; i++)
            {
                TableCell cell = new TableCell(dc);
                cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, BorderStyle.ThickThinSmallGap, Color.Gray, 1.0);
                cell.CellFormat.PreferredWidth = new TableWidth(width / numberOfSelectedCols, TableWidthUnit.Point);
                Paragraph text = new Paragraph(dc);
                text.Content.Start.Insert(stringLabels[i], new CharacterFormat() { FontColor = new Color("#607494"), Size = 8.0, Bold = true });
                cell.Blocks.Add(text);
                firstRow.Cells.Add(cell);
            }

            table.Rows.Add(firstRow);


            foreach (var allInfoForDevice in allInfoForDevices)
            {
                Paragraph name = new Paragraph(dc);
                name.Content.Start.Insert(allInfoForDevice.Device.Name, new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

                Paragraph location = new Paragraph(dc);
                location.Content.Start.Insert(allInfoForDevice.Device.Location, new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

                Paragraph latitude = new Paragraph(dc);
                latitude.Content.Start.Insert(allInfoForDevice.Device.LocationLatitude.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

                Paragraph longitude = new Paragraph(dc);
                longitude.Content.Start.Insert(allInfoForDevice.Device.LocationLongitude.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

                Paragraph status = new Paragraph(dc);
                status.Content.Start.Insert(allInfoForDevice.Device.Status.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

                Paragraph lastTimeOnline = new Paragraph(dc);
                lastTimeOnline.Content.Start.Insert(allInfoForDevice.Device.LastTimeOnline.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

                Paragraph ram = new Paragraph(dc);
                Paragraph gpu = new Paragraph(dc);
                Paragraph cpu = new Paragraph(dc);
                Paragraph hdd = new Paragraph(dc);

                if (allInfoForDevice.Averages == null)
                {
                    ram.Content.Start.Insert("Nema podataka o RamUsage", new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                    gpu.Content.Start.Insert("Nema podataka o RamUsage", new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                    cpu.Content.Start.Insert("Nema podataka o RamUsage", new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                    hdd.Content.Start.Insert("Nema podataka o RamUsage", new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                }
                else
                {
                    ram.Content.Start.Insert(allInfoForDevice.Averages.RamUsage.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                    gpu.Content.Start.Insert(allInfoForDevice.Averages.Gpuusage.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                    cpu.Content.Start.Insert(allInfoForDevice.Averages.CpuUsage.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                    hdd.Content.Start.Insert(allInfoForDevice.Averages.Hddusage.ToString(), new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });
                }

                Paragraph groupName = new Paragraph(dc);
                groupName.Content.Start.Insert(selectedGroup.Name, new CharacterFormat() { FontColor = new Color("#8a8d91"), Size = 8.0, Bold = true });

                TableRow dataRow = new TableRow(dc);
                foreach (var label in queryModel.select)
                {
                    TableCell dataCell = new TableCell(dc);
                    dataCell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, BorderStyle.ThickThinSmallGap, Color.Gray, 1.0);
                    dataCell.CellFormat.PreferredWidth = new TableWidth(width / numberOfSelectedCols, TableWidthUnit.Point);

                    switch (label)
                    {
                        case "name": dataCell.Blocks.Add(name); break;
                        case "avgRamUsage": dataCell.Blocks.Add(ram); break;
                        case "avgGpuUsage": dataCell.Blocks.Add(gpu); break;
                        case "quarterlyCpuUsage": dataCell.Blocks.Add(cpu); break;
                        case "diskUtilization": dataCell.Blocks.Add(hdd); break;
                        case "location": dataCell.Blocks.Add(location); break;
                        case "latitude": dataCell.Blocks.Add(latitude); break;
                        case "longitude": dataCell.Blocks.Add(longitude); break;
                        case "status": dataCell.Blocks.Add(status); break;
                        case "groupName": dataCell.Blocks.Add(groupName); break;
                    }

                    dataRow.Cells.Add(dataCell);
                }

                table.Rows.Add(dataRow);

            }

            s.Blocks.Add(table);

            dc.Save(documentPath, new PdfSaveOptions() { Compliance = PdfCompliance.PDF_A1a });

            return dc;
        }
    }
}
