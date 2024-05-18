using System;
using System;
using System.Collections.Generic;
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

        // Doldurulmuş verileri yazdır
        foreach (var data in vaccinationData)
        {
            Console.WriteLine($"{data.Country}, {data.Date}, {data.DailyVaccinations}, {data.Vaccines}");
        }

        WriteDataToFile(filePath, vaccinationData); // Veriler burada dosyaya da yazılır.

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
                        VaccinationData data = new VaccinationData
                        {
                            Country = parts[0],
                            Date = parts[1],
                            // daily_vaccinations sütununu kontrol ediyoruz
                            DailyVaccinations = string.IsNullOrEmpty(parts[2]) ? 0 : int.Parse(parts[2]),
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

    static void WriteDataToFile(string filePath, List<VaccinationData> vaccinationData)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                // Başlık satırını yeniden yaz
                sw.WriteLine("country,date,daily_vaccinations,vaccines");

                // Güncellenmiş verileri dosyaya yaz
                foreach (var data in vaccinationData)
                {
                    sw.WriteLine($"{data.Country},{data.Date},{data.DailyVaccinations},{data.Vaccines}");
                }
            }

            Console.WriteLine("Veriler başarıyla dosyaya yazıldı.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosyaya yazma işlemi sırasında bir hata oluştu: {ex.Message}");
        }
    }


}

class VaccinationData
{
    public string Country { get; set; }
    public string Date { get; set; }
    public int DailyVaccinations { get; set; }
    public string Vaccines { get; set; }



}
