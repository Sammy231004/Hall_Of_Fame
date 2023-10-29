using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Diagnostics;
using Telegram.Bot.Extensions.Session;
using Telegram.Bot.Extensions.Session.Stores;
using Telegram.Bot.Types.Enums;

public class Program
{
    private static TelegramBotClient botClient;
    private static readonly string ADMIN_CHAT_ID = "6048408201"; // Замените на свой Chat ID
    private static HttpClient httpClient = new HttpClient();

    public static async Task Main()
    {
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        Trace.AutoFlush = true;
        botClient = new TelegramBotClient("6405023100:AAGmH4Xw5khvRDBYnVPli-bQRsLViZTgo3U"); // Замените на свой токен бота

        botClient.OnMessage += BotOnMessageReceived;

        // Добавьте эту строку, чтобы включить использование сессий
        botClient.UseSessionState();

        botClient.StartReceiving();
        Trace.TraceInformation("Bot started.");

        Console.WriteLine("Bot started. Press Enter to exit.");
        Console.ReadLine();

        botClient.StopReceiving();
    }

    private static async void BotOnMessageReceived(object sender, MessageEventArgs e)
    {
        var message = e.Message;

        if (message == null || message.Type != MessageType.Text)
            return;

        var userText = message.Text;
        var userId = message.Chat.Id;

        // Получение состояния сессии для пользователя
        var sessionState = e.GetSessionState();

        if (userText.StartsWith("/start"))
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Расписание"), new KeyboardButton("Оценки") }
            });

            await botClient.SendTextMessageAsync(userId, "Привет! Для доступа к расписанию, введите свой логин и пароль через пробел.", replyMarkup: keyboard);
        }
        else
        {
            if (sessionState == null)
            {
                sessionState = new SessionState();
                e.SetSessionState(sessionState);
            }

            if (sessionState.ContainsKey("auth") && sessionState["auth"] == "true")
            {
                if (userText.Equals("Расписание", StringComparison.OrdinalIgnoreCase))
                {
                    await GetScheduleAsync(userId);
                }
                else if (userText.Equals("Оценки", StringComparison.OrdinalIgnoreCase))
                {
                    await GetGradesAsync(userId);
                }
                else
                {
                    await botClient.SendTextMessageAsync(userId, "Используйте кнопки меню или введите 'Оценки' или 'Расписание'.");
                }
            }
            else
            {
                var loginPassword = userText.Split(' ');
                if (loginPassword.Length == 2)
                {
                    var login = loginPassword[0];
                    var password = loginPassword[1];
                    Trace.TraceError($"Ошибка аутентификации пользователя {userId}: Аутентификация не удалась для пользователя {userId}");
                    if (await AuthenticateUserAsync(userId, login, password))
                    {
                        sessionState["auth"] = "true";
                        var keyboard = new ReplyKeyboardMarkup(new[]
                        {
                            new[] { new KeyboardButton("Расписание"), new KeyboardButton("Оценки") }
                        });

                        await botClient.SendTextMessageAsync(userId, "Аутентификация успешна! Теперь вы можете посмотреть свои оценки или расписание :)", replyMarkup: keyboard);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(userId, "Не удалось аутентифицироваться. Проверьте логин и пароль и попробуйте снова.");
                    }
                }
                else
                {
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[] { new KeyboardButton("Расписание"), new KeyboardButton("Оценки") }
                    });

                    await botClient.SendTextMessageAsync(userId, "Введите логин и пароль через пробел.", replyMarkup: keyboard);
                }
            }
        }
    }

    private static async Task<bool> AuthenticateUserAsync(long userId, string login, string password)
    {
        try
        {
            var loginData = new
            {
                login = login,
                password = HashAndBase64Encode(password),
                isRemember = true
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Trace.TraceInformation($"Authenticating user {userId} with login {login}");

            var response = await httpClient.PostAsync("https://poo.tomedu.ru/services/security/login", content);

            if (response.IsSuccessStatusCode)
            {
                Trace.TraceInformation($"Authentication successful for user {userId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeAnonymousType(responseContent, new { tenants = new Dictionary<string, object>() });

                foreach (var tenant in data.tenants)
                {
                    if (tenant.Key.StartsWith("SPO_") && tenant.Value is Dictionary<string, object> tenantData)
                    {
                        if (tenantData.TryGetValue("studentRole", out var studentRole) && studentRole is Dictionary<string, object> studentRoleData)
                        {
                            if (studentRoleData.TryGetValue("id", out var studentId))
                            {
                                var studentIdStr = studentId.ToString();
                                studentIds[userId] = studentIdStr;
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                Trace.TraceInformation($"Authentication failed for user {userId}: Status Code {response.StatusCode}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error during authentication for user {userId}: {ex.Message}");
            return false;
        }
    }

    private static string HashAndBase64Encode(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Convert.ToBase64String(hashBytes);
            Console.WriteLine("Hashed Password: " + hashedPassword);
            return hashedPassword;
        }
    }

    private static async Task GetScheduleAsync(long userId)
    {
        if (studentIds.TryGetValue(userId, out var studentId))
        {
            var currentDate = DateTime.Now;
            var endDate = currentDate.AddDays(10); // Adjust the date range as needed

            var scheduleUrl = $"https://poo.tomedu.ru/services/students/{studentId}/lessons/{currentDate:yyyy-MM-dd}/{endDate:yyyy-MM-dd}";
            var response = await httpClient.GetAsync(scheduleUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var scheduleData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseContent);

                var messages = ProcessScheduleData(scheduleData);

                if (messages.Any())
                {
                    foreach (var message in messages)
                    {
                        await botClient.SendTextMessageAsync(userId, message);
                    }

                    await botClient.SendTextMessageAsync(ADMIN_CHAT_ID, $"Пользователь {userId} просмотрел расписание.");
                }
                else
                {
                    await botClient.SendTextMessageAsync(userId, "На данной странице нет расписания.");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(userId, "Не удалось загрузить расписание. Попробуйте позже.");
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(userId, "Сначала авторизуйтесь, чтобы получить расписание.");
            await botClient.SendTextMessageAsync(ADMIN_CHAT_ID, "Пользователь запросил расписание, но не авторизован.");
        }
    }

    private static List<string> ProcessScheduleData(List<Dictionary<string, object>> scheduleData)
    {
        var messages = new List<string>();

        foreach (var day in scheduleData)
        {
            if (day.TryGetValue("date", out var dateObj) && dateObj is string dateStr)
            {
                var date = DateTime.Parse(dateStr);
                var formattedDate = date.ToString("yyyy-MM-dd (dddd)", System.Globalization.CultureInfo.InvariantCulture);
                messages.Add($"Дата: {formattedDate}");

                if (day.TryGetValue("lessons", out var lessonsObj) && lessonsObj is List<Dictionary<string, object>> lessons)
                {
                    foreach (var lesson in lessons)
                    {
                        if (lesson.TryGetValue("name", out var nameObj) && nameObj is string name)
                        {
                            messages.Add(string.Empty);
                            var startTime = lesson.TryGetValue("startTime", out var startTimeObj) ? startTimeObj.ToString() : "Время не указано";
                            var endTime = lesson.TryGetValue("endTime", out var endTimeObj) ? endTimeObj.ToString() : "Время не указано";

                            messages.Add($"⏰: {startTime} - {endTime}");
                            messages.Add($"📚: {name}");

                            if (lesson.TryGetValue("timetable", out var timetableObj) && timetableObj is Dictionary<string, object> timetable)
                            {
                                var classroomName = timetable.TryGetValue("classroom", out var classroomObj) ? classroomObj.ToString() : "Не указана";
                                var teacherName = timetable.TryGetValue("teacher", out var teacherObj) ? $"{teacherObj}" : "Не указан";

                                messages.Add($"🚪: {classroomName}");
                                messages.Add($"👨‍🏫: {teacherName}");
                            }
                        }
                    }
                }
            }
        }

        return messages;
    }

    private static async Task GetGradesAsync(long userId)
    {
        if (studentIds.TryGetValue(userId, out var studentId))
        {
            var gradesUrl = $"https://poo.tomedu.ru/services/reports/current/performance/{studentId}";
            var response = await httpClient.GetAsync(gradesUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var gradesData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

                var gradesText = GenerateGradesText(gradesData);

                await botClient.SendTextMessageAsync(userId, gradesText);

                await botClient.SendTextMessageAsync(ADMIN_CHAT_ID, $"Пользователь {userId} просмотрел оценки.");
            }
            else
            {
                await botClient.SendTextMessageAsync(userId, "Не удалось загрузить оценки. Попробуйте позже.");
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(userId, "Сначала авторизуйтесь, чтобы получить оценки.");
        }
    }

    private static string GenerateGradesText(Dictionary<string, object> gradesData)
    {
        var gradesText = "Оценки:\n";

        if (gradesData.TryGetValue("daysWithMarksForSubject", out var subjectGradesObj) && subjectGradesObj is List<Dictionary<string, object>> subjectGrades)
        {
            foreach (var subject in subjectGrades)
            {
                var subjectNameStr = subject.TryGetValue("subjectName", out var subjectNameObj) ? subjectNameObj.ToString() : "Не указано";
                gradesText += $"{subjectNameStr}: ";

                if (subject.TryGetValue("daysWithMarks", out var daysWithMarksObj) && daysWithMarksObj is List<Dictionary<string, object>> daysWithMarksList)
                {
                    var markValuesList = new List<string>();
                    foreach (var dayWithMarks in daysWithMarksList)
                    {
                        var absenceType = dayWithMarks.TryGetValue("absenceType", out var absenceTypeObj) ? absenceTypeObj.ToString() : null;
                        var markValuesObj = dayWithMarks.TryGetValue("markValues", out var markValuesListObj) ? markValuesListObj : null;

                        if (markValuesObj is List<object> markValuesListTemp)
                        {
                            markValuesList.AddRange(markValuesListTemp.Select(v => v.ToString()));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(absenceType))
                            {
                                markValuesList.Add("H");
                            }
                        }
                    }

                    gradesText += string.Join(" ", markValuesList);
                }
                else
                {
                    gradesText += "Оценок нет";
                }

                gradesText += "\n";
            }
        }
        else
        {
            gradesText = "Нет данных об оценках";
        }

        return gradesText;
    }
}
