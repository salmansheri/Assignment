
using Assignment.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReverseWordController : ControllerBase 
    {
        [HttpPost]
        public ActionResult<ReverseWord> ReverseWord([FromBody] ReverseWord reverseWord)
        {
            if (reverseWord == null)
            {
                return BadRequest();
            }

            string[] words = reverseWord.Word.Split(' ');
            Array.Reverse(words);
            var result = string.Join(" ", words); 

            return Ok(result);

        } 

    }
}