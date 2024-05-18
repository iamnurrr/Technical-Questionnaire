using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        // Dosya yolunu güncelleyin
        string filePath = @"C:\\Users\\x\\Downloads\\country_vaccination_stats.csv";

        // Verileri oku
        List<VaccinationData> vaccinationData = ReadData(filePath);

        // Eksik verileri doldur
        FillMissingData(vaccinationData);

        // Belirtilen tarihte yapılan toplam aşılama sayısını hesapla
        DateTime targetDate = DateTime.ParseExact("01/06/2021", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        int totalVaccinationsOnDate = CalculateTotalVaccinationsOnDate(vaccinationData, targetDate);

        // Sonucu yazdır
        Console.WriteLine(totalVaccinationsOnDate);
    }

    static List<VaccinationData> ReadData(string filePath)
    {
        List<VaccinationData> vaccinationData = new List<VaccinationData>();

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                // Başlık satırını oku ve atla
                sr.ReadLine();

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 4)
                    {
                        DateTime date = DateTime.ParseExact(parts[1], "M/d/yyyy", CultureInfo.InvariantCulture);
                        int dailyVaccinations = string.IsNullOrEmpty(parts[2]) ? 0 : int.Parse(parts[2]);

                        VaccinationData data = new VaccinationData
                        {
                            Country = parts[0],
                            Date = date,
                            DailyVaccinations = dailyVaccinations,
                            Vaccines = parts[3]
                        };
                        vaccinationData.Add(data);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }

        return vaccinationData;
    }

    static void FillMissingData(List<VaccinationData> vaccinationData)
    {
        var countries = vaccinationData.Select(v => v.Country).Distinct().ToList();

        foreach (var country in countries)
        {
            var countryData = vaccinationData.Where(v => v.Country == country && v.DailyVaccinations > 0).ToList();
            var minDailyVaccinations = countryData.Any() ? countryData.Min(v => v.DailyVaccinations) : 0;

            foreach (var data in vaccinationData.Where(v => v.Country == country && v.DailyVaccinations == 0))
            {
                data.DailyVaccinations = minDailyVaccinations;
            }
        }
    }

    static int CalculateTotalVaccinationsOnDate(List<VaccinationData> vaccinationData, DateTime date)
    {
        return vaccinationData
            .Where(v => v.Date == date)
            .Sum(v => v.DailyVaccinations);
    }

    class VaccinationData
    {
        public string Country { get; set; }
        public DateTime Date { get; set; }
        public int DailyVaccinations { get; set; }
        public string Vaccines { get; set; }
    }
}
