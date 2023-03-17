using System.Text.RegularExpressions;
using DatabaseSetupLocal.Models;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace DatabaseSetupLocal.Rep;

public static class F1WebScraper
{
    public static List<string> GetRaceResults(int year, int raceNumber)
    {
        var noRace = 1124 + raceNumber;
        var results = new List<string>();
        var url = $"https://www.formula1.com/en/results.html/{year}/races/{noRace}/race-result.html";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        var tableXPath = "//tr";
        var resultsNodes = doc.DocumentNode.SelectNodes(tableXPath).Skip(1)
            .Select(x => x.ChildNodes[7].ChildNodes[5].InnerHtml);
        //doc.DocumentNode.SelectNodes("//tr")[1].ChildNodes[3]
        foreach (var resultsNode in resultsNodes)
        {
            results.Add(resultsNode);
        }

        return results;
    }

    // https://www.formula1.com/en/racing/2023.html
    public static List<string> GetCountryListOfRaces(int year)
    {
        var url = $"https://www.formula1.com/en/racing/{year}.html";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        var xPath = "//*[contains(@class, 'event-place')]";
        var result = doc.DocumentNode.SelectNodes(xPath)
            .Select(x => x.ChildNodes.First().InnerHtml.TrimEnd()).ToList();

        return result;
    }

    public static List<string> GetDatesListOfRaces(int year)
    {
        var url = $"https://www.formula1.com/en/racing/{year}.html";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        var startXPath = "//*[contains(@class, 'start-date')]";
        var endXPath = "//*[contains(@class, 'start-date')]";
        var monthXPath = "//*[contains(@class, 'month-wrapper f1-wide--xxs')]";
        var resultStart = doc.DocumentNode.SelectNodes(startXPath)
            .Select(x => x.ChildNodes.First().InnerHtml.TrimEnd()).ToList();
        var resultEnd = doc.DocumentNode.SelectNodes(endXPath)
            .Select(x => x.ChildNodes.First().InnerHtml.TrimEnd()).ToList();
        var resultMonth = doc.DocumentNode.SelectNodes(monthXPath)
            .Select(x => x.ChildNodes.First().InnerHtml.TrimEnd()).ToList();
        var result = new List<string>();
        return result;
    }

    public static F1Grid GetDriversData()
    {
        var url = $"https://www.formula1.com/en/results.html/2023/races/1141/bahrain/race-result.html";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        var fullNameXPath = "//*[contains(@class, 'dark bold')]";
        var numberXPath = "//*[contains(@class, 'dark hide-for-mobile')]";
        var teamXPath = "//*[contains(@class, 'semi-bold uppercase hide-for-tablet')]";
        var resultFullName = doc.DocumentNode.SelectNodes(fullNameXPath).Where((x, i) => i % 2 == 0)
            .Select(x => x.ChildNodes.Where((val, i) => i % 2 != 0).Select(x => x.InnerHtml).Take(2)).ToList();
        var resultFirstName = doc.DocumentNode.SelectNodes(fullNameXPath).Where((x, i) => i % 2 == 0)
            .Select(x => x.ChildNodes.Where((val, i) => i % 2 != 0).Select(x => x.InnerHtml).First()).ToList();
        var resultLastName = doc.DocumentNode.SelectNodes(fullNameXPath).Where((x, i) => i % 2 == 0).Select(x =>
            x.ChildNodes.Where((val, i) => i % 2 != 0).Select(x => x.InnerHtml).Skip(1).First()).ToList();
        var resultAbb = doc.DocumentNode.SelectNodes(fullNameXPath).Where((x, i) => i % 2 == 0).Select(x =>
            x.ChildNodes.Where((val, i) => i % 2 != 0).Select(x => x.InnerHtml).Skip(2).First()).ToList();
        var resultNumber = doc.DocumentNode.SelectNodes(numberXPath)
            .Select(x => x.ChildNodes.Select(x => x.InnerHtml).First()).ToList();
        var resultTeam = doc.DocumentNode.SelectNodes(teamXPath).Select(x => x.InnerHtml).ToList();
        var f1Grid = new F1Grid();
        f1Grid.Year = 2023;
        var driversList = new List<Driver>();
        for (int i = 0; i < 20; i++)
        {
            var driver = new Driver();
            driver.Number = resultNumber[i];
            driver.FirstName = resultFirstName[i];
            driver.LastName = resultLastName[i];
            driver.Abbreviation = resultAbb[i];
            driver.Team = resultTeam[i];
            driver.FullName = resultFirstName[i] + " " + resultLastName[i];
            driversList.Add(driver);
        }

        f1Grid.Drivers = driversList;

        return f1Grid;
    }

    public static F1Schedule GetScheduleData()
    {
        var url = $"https://f1calendar.com";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        var xPath = "//*[contains(@class, 'w-full')]";

        var resultRaces = doc.DocumentNode.SelectNodes(xPath).Skip(2).Select(x =>
                x.ChildNodes.Skip(1)
                    .Select(x => x.ChildNodes.Take(1).Select(x => x.ChildNodes.Skip(1).First().InnerText).First())
                    .ToList())
            .First();

        var resultEvents = doc.DocumentNode.SelectNodes(xPath).Skip(2).Select(x =>
            x.ChildNodes.Skip(1).Select(x =>
                x.ChildNodes.Skip(1).Select(x => x.ChildNodes.Skip(1).First().ChildNodes.Skip(3).First().InnerText)
                    .ToList())).First().ToList();

        var resultDates = doc.DocumentNode.SelectNodes(xPath).Skip(2).Select(x =>
                x.ChildNodes.Skip(1).Select(x => x.ChildNodes.Skip(1).Select(x =>
                        x.ChildNodes.Skip(2).First().InnerHtml + " 2023 " + x.ChildNodes.Skip(3).First().InnerText)
                    .ToList()))
            .First().ToList();

        var resultTimes = doc.DocumentNode.SelectNodes(xPath).Skip(2).Select(x =>
            x.ChildNodes.Skip(1).Select(x =>
                x.ChildNodes.Skip(1).Select(x => x.ChildNodes.Skip(3).First().InnerText).ToList())).First().ToList();

        var f1Schedule = new F1Schedule();
        f1Schedule.Year = 2023;
        var raceScheduleList = new List<RaceSchedule>();

        for (int i = 0; i < 23; i++)
        {
            var raceSchedule = new RaceSchedule();
            var eventList = new List<F1Event>();
            raceSchedule.RaceName = resultRaces[i];

            for (int j = 0; j < 5; j++)
            {
                var f1Event = new F1Event();
                f1Event.EventName = resultEvents[i][j];
                f1Event.EventDateAndTime = Convert.ToDateTime(resultDates[i][j]);
                eventList.Add(f1Event);
            }

            raceSchedule.F1Events = eventList;
            raceScheduleList.Add(raceSchedule);
        }

        f1Schedule.Races = raceScheduleList;
        return f1Schedule;
    }

    public static List<string> GetLiveData()
    {
        var url = $"https://live.planetf1.com/liverace";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        var pattern = @"[^\W\d](\w|[-']{1,2}(?=\w))*";
        var rg = new Regex(pattern);
        var xPath = "/html/body/div[3]/div/main/section/div/div/div[1]/div[1]/div[4]/table/tbody/tr[1]/td[3]/h2/a";
        var resultStart = doc.DocumentNode.SelectNodes("//*[contains(@class, 'signalr_live_race_html')]").First()
            .ChildNodes.Where((value, index) => index % 2 != 0)
            .Select(x => x.ChildNodes[5].ChildNodes[1].ChildNodes[1].InnerText).ToList();

        var res = resultStart.Select(x => rg.Matches(x));
        var result = res.Select(x => x.Select(x => x.Value).ToList().Aggregate((i, j) => i + " " + j)).ToList();


        return result;
    }
}