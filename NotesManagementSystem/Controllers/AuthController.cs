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

namespace NotesManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJWTAuthenticationManager _jWTAuthenticationManager;

        public AuthController(IJWTAuthenticationManager jWTAuthenticationManager)
        {
            _jWTAuthenticationManager = jWTAuthenticationManager;
        }
        private readonly Random _random = new Random();
        private readonly string path = Path.Combine(Environment.CurrentDirectory, @"Data/Users.json");


        [HttpPost("VerifyUserLogin")]
        public IActionResult VerifyUserLogin([FromBody] User user)
        {
            object returnObj = new
            {
                RetCode = "0",
                RetMsg = "notOk",
                UserId="",
                UserName = "",
                Email = "",
                DateOfBith = "",
                token = ""
            };
            try
            {
                var json = System.IO.File.ReadAllText(path);
                List<User> notes = JsonConvert.DeserializeObject<List<User>>(json);

                User isUser = notes.Where(x => x.Email == user.Email && x.Password == user.Password).SingleOrDefault();

                if(isUser != null)
                {
                    var token = _jWTAuthenticationManager.Authenticate(user.Email);

                    returnObj = new
                    {
                        RetCode = "1",
                        RetMsg = "ok",
                        UserId=isUser.Id,
                        UserName = isUser.Name,
                        Email = isUser.Email,
                        DateOfBirth = isUser.DateOfBirth,
                        token = token
                    };
                    return Ok(returnObj);
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }

            return Ok(returnObj);
        }

        [HttpPost("UserSignup")]
        public IActionResult UserSignup([FromBody] User user)
        {
            object returnObj = new
            {
                RetCode = "0",
                RetMsg = "notOk",
            };
            try
            {
                if(user.Email != "" && user.Password != "")
                {
                    var json = System.IO.File.ReadAllText(path);      //existing data

                    #region check_if_email_exists
                    List<User> notes = JsonConvert.DeserializeObject<List<User>>(json);

                    User isUser = notes.Where(x => x.Email == user.Email).SingleOrDefault();
                    if(isUser != null)
                    {
                        returnObj = new
                        {
                            RetCode = "0",
                            RetMsg = "Email already exists"
                        };
                        return Ok(returnObj);
                    }
                    #endregion

                    int random = _random.Next(100000, 999999);
                    var newUser = "{ 'id': " + random + ",'name': '" + user.Name+ "','email': '" + user.Email+"','password': '"+user.Password+ "','dateOfBirth': '" + user.DateOfBirth + "'}";
                    
                    var jsonObj = JArray.Parse(json);    // existing data parsing
                    var newUserObj = JObject.Parse(newUser);     // new user object parsing 

                    jsonObj.Add(newUserObj);

                    string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                           Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(path, newJsonResult);

                    returnObj = new
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
    }
}
