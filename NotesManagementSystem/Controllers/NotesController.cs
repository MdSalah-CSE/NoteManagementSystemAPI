using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using NotesManagementSystem.Model;
using LGuardaService.API.JWTAuth;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace NotesManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly CultureInfo dateformat = System.Globalization.CultureInfo.GetCultureInfo("hi-IN");
        private readonly string path = Path.Combine(Environment.CurrentDirectory, @"Data/Notes.json");
       
        private readonly Random _random = new Random();


        #region Get

        //API for return reminders by Day/Week/Month 
        [Authorize]
        [HttpGet("GetRemindersForDayWeekMonth")]
        public IActionResult GetRemindersForDayWeekMonth(int type, int userId)
        {
            try
            {
                var json = System.IO.File.ReadAllText(path);
                List<Note> notes = JsonConvert.DeserializeObject<List<Note>>(json);

                List<Note> Reminders = new List<Note>();

                foreach (var item in notes)
                {
                    if (type == 1)  // for day
                    {
                        if (item.UserId == userId && item.Type == 2 && Convert.ToDateTime(item.ReminderDateTime, dateformat.DateTimeFormat).Date == DateTime.Now.Date)
                        {
                            Reminders.Add(item);
                        }
                    }
                    else if (type == 2)  // for week 
                    {
                        if (item.UserId == userId && item.Type == 2 && Convert.ToDateTime(item.ReminderDateTime, dateformat.DateTimeFormat).Date >= DateTime.Now.Date
                            && Convert.ToDateTime(item.ReminderDateTime, dateformat.DateTimeFormat).Date < DateTime.Now.Date.AddDays(7))         //This week = Today + Next 6days
                        {
                            Reminders.Add(item);
                        }
                    }
                    else  // for month
                    {
                        if (item.UserId == userId && item.Type == 2 && Convert.ToDateTime(item.ReminderDateTime, dateformat.DateTimeFormat).Month == DateTime.Now.Month)
                        {
                            Reminders.Add(item);
                        }
                    }

                }

                return Ok(Reminders);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok();
        }


        //API for return tasks/todo by Day/Week/Month 
        [Authorize]
        [HttpGet("GetTasksForDayWeekMonth")]
        public IActionResult GetTasksForDayWeekMonth(int type, int userId)
        {
            try
            {

                var json = System.IO.File.ReadAllText(path);
                List<Note> notes = JsonConvert.DeserializeObject<List<Note>>(json);

                List<Note> tasks = new List<Note>();

                foreach (var item in notes)
                {
                    if (type == 1)   // for today
                    {
                        if (item.UserId == userId && item.Type == 3 && Convert.ToDateTime(item.DueDate, dateformat.DateTimeFormat).Date == DateTime.Now.Date)
                        {
                            tasks.Add(item);
                        }
                    }
                    else if (type == 2)   //for week
                    {
                        if (item.UserId == userId && item.Type == 3 && Convert.ToDateTime(item.DueDate, dateformat.DateTimeFormat).Date >= DateTime.Now.Date
                            && Convert.ToDateTime(item.DueDate, dateformat.DateTimeFormat).Date < DateTime.Now.Date.AddDays(7))
                        {
                            tasks.Add(item);
                        }
                    }
                    else                 // for month
                    {
                        if (item.UserId == userId && item.Type == 3 && Convert.ToDateTime(item.DueDate, dateformat.DateTimeFormat).Month == DateTime.Now.Month)
                        {
                            tasks.Add(item);
                        }
                    }

                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok();
        }



        [Authorize]
        [HttpGet("ChangeTaskStatusById")]
        public IActionResult ChangeTaskStatusById(int taskId)
        {
            try
            {
                var json = System.IO.File.ReadAllText(path);
                var jsonObj = JArray.Parse(json);

                if (taskId != 0)
                {
                    foreach (var task in jsonObj.Where(obj => obj["id"].Value<int>() == taskId))
                    {
                        task["taskStatus"] = "1";
                    }

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(path, output);
                }
                else
                {
                    return NotFound("Invalid Task ID, Try Again!");
                }


                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok();
        }


        [Authorize]
        [HttpGet("GetNoteByType")]
        public IActionResult GetNoteByType(int type, int userId)
        {
            try
            {
                var json = System.IO.File.ReadAllText(path);
                List<Note> notes = JsonConvert.DeserializeObject<List<Note>>(json);

                List<Note> finalNotes = notes.Where(x => x.Type == type && x.UserId == userId).ToList();

                return Ok(finalNotes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok();
        }



        [Authorize]
        [HttpGet("GetCountOfNotesByType")]
        public IActionResult GetCountOfNotesByType(int type, int userId)
        {
            try
            {
                var json = System.IO.File.ReadAllText(path);
                List<Note> notes = JsonConvert.DeserializeObject<List<Note>>(json);

                List<Note> finalNotes = notes.Where(x => x.Type == type && x.UserId == userId).ToList();

                return Ok(finalNotes.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("GetCountOfIndividualNotes")]
        public IActionResult GetCountOfIndividualNotes(int userId)
        {
            try
            {
                var json = System.IO.File.ReadAllText(path);
                List<Note> notes = JsonConvert.DeserializeObject<List<Note>>(json).Where(x => x.UserId == userId).ToList();


                List<object> counts = new List<object>();
                object countOfRegularNotes = new { type = 1, count = notes.Where(x => x.Type == 1).ToList().Count };
                object countOfReminders = new { type = 2, count = notes.Where(x => x.Type == 2).ToList().Count };
                object countOfTasks = new { type = 3, count = notes.Where(x => x.Type == 3).ToList().Count };
                object countOfBookmarks = new { type = 4, count = notes.Where(x => x.Type == 4).ToList().Count };

                counts.Add(countOfRegularNotes);
                counts.Add(countOfReminders);
                counts.Add(countOfTasks);
                counts.Add(countOfBookmarks);

                return Ok(counts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok();
        }
        #endregion

        #region Post

        [Authorize]
        [HttpPost("PostNote")]
        public IActionResult PostNote([FromBody] Note note)
        {
            try
            {
                if(note.UserId != null && note.Type != null)
                {
                    note.MakeDate = DateTime.Now.ToString();
                    int random = _random.Next(100000, 999999);
                    var newNote = "{ 'id': " + random + ",'userId': " + note.UserId + ",'type': '" + note.Type + "','text': '" + note.Text + "','reminderDateTime': '" + note.ReminderDateTime +
                        "','dueDate': '" + note.DueDate + "','taskStatus': '" + note.TaskStatus + "','webURL': '" + note.WebURL + "','makeDate': '" + note.MakeDate + "'}";

                    var json = System.IO.File.ReadAllText(path);
                    var jsonObj = JArray.Parse(json);
                    var newUserObj = JObject.Parse(newNote);

                    jsonObj.Add(newUserObj);

                    string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                           Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(path, newJsonResult);


                    object returnObj = new
                    {
                        RetCode = "1",
                        RetMsg = "ok",
                    };

                    return Ok(returnObj);
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok();
        }


        #endregion

    }
}
