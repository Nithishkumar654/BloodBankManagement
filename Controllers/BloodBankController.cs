using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BloodBank.Models;

namespace BloodBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodBankController : ControllerBase
    {
        static List<BloodBankEntry> entries = new List<BloodBankEntry>
        {
            new BloodBankEntry {Id=Guid.NewGuid().ToString(), DonorName="Nithish", Age=20, BloodType="O+", PhoneNumber="7416947874", Email="nithishkumar.s654@gmail.com", Quantity=50, CollectionDate=DateTime.Now, ExpirationDate=DateTime.Now.AddDays(10), Status="Available"},
            new BloodBankEntry {Id=Guid.NewGuid().ToString(), DonorName="Mukesh", Age=22, BloodType="AB+", PhoneNumber="7788778787", Email="mukesh@gmail.com", Quantity=100, CollectionDate=new DateTime(2024, 10, 10), ExpirationDate=new DateTime(2024, 10, 10).AddDays(10), Status="Expired"},
            new BloodBankEntry {Id=Guid.NewGuid().ToString(), DonorName="Satish", Age=25, BloodType="AB-", Email="satish@gmail.com", Quantity=70, CollectionDate=new DateTime(2024, 11, 25), ExpirationDate=new DateTime(2024, 11, 25).AddDays(10), Status="Requested"},
            new BloodBankEntry {Id=Guid.NewGuid().ToString(), DonorName="Naveen", Age=29, BloodType="B+", PhoneNumber="9392662041", Quantity=100, CollectionDate=new DateTime(2024, 11, 15), ExpirationDate=new DateTime(2024, 11, 15).AddDays(10), Status="Available"},
            new BloodBankEntry {Id=Guid.NewGuid().ToString(), DonorName="Sreya", Age=21, BloodType="O+", Email="sreya@gmail.com", Quantity=50, CollectionDate=new DateTime(2024, 8, 8), ExpirationDate=new DateTime(2024, 8, 8).AddDays(10), Status="Expired"},
        };


        // Get All Entries
        [HttpGet("GetEntries")]
        public ActionResult<IEnumerable<BloodBankEntry>> GetEntries()
        {
            if (!entries.Any()) return NotFound();
            return entries;
        }


        // GetById Entry
        [HttpGet("GetEntry/{id}")]
        public ActionResult<BloodBankEntry> GetEntry(string id)
        {
            
            var res = entries.Find(i => i.Id == id);

            if (res == null) return NotFound();

            return res;
        }


        string validate(BloodBankEntry entry)
        {
            if (entry.DonorName == null || entry.Age == null || entry.BloodType == null || entry.Quantity == null || entry.CollectionDate == null || entry.ExpirationDate == null)
                return "Required Fields.. Please fill DonorName, Age, BloodType, Quantity(ml), CollectionDate, ExpirationDate..";

            if (entry.Age < 18) return "Donor must be atleast 18 years old";
            if (entry.BloodType.Equals("AB+", StringComparison.OrdinalIgnoreCase) || entry.BloodType.Equals("AB-", StringComparison.OrdinalIgnoreCase) || entry.BloodType.Equals("O+", StringComparison.OrdinalIgnoreCase) || entry.BloodType.Equals("O-", StringComparison.OrdinalIgnoreCase) || entry.BloodType.Equals("B+", StringComparison.OrdinalIgnoreCase) || entry.BloodType.Equals("B-", StringComparison.OrdinalIgnoreCase) || entry.BloodType.Equals("A+", StringComparison.OrdinalIgnoreCase) || entry.BloodType.Equals("A-", StringComparison.OrdinalIgnoreCase))
            {

            }
            else
            {
                return "Invalid Blood Type";
            }

            if (entry.PhoneNumber == null && entry.Email == null)
            {
                return "Either PhoneNumber or Email must be provided";
            }
            if (entry.PhoneNumber != null && entry.PhoneNumber.Length > 0 && entry.PhoneNumber.Length != 10)
            {
                return "Contact Number must be of 10 digits";
            }

            if (entry.Quantity <= 0)
                return "Quantity can't be Non-Positive.";

            if (entry.CollectionDate < DateTime.Today)
                return "Collection Date can't be Past Days";

            if (entry.ExpirationDate < entry.CollectionDate)
                return "Expiration Date can't be before Collection Date";

            return "success";
        }


        // Post Entry
        [HttpPost("InsertEntry")]
        public ActionResult<BloodBankEntry> InsertEntry(BloodBankEntry entry)
        {
            if(entry == null) return BadRequest();

            string val = validate(entry);

            if (val != "success") return BadRequest(val);

            entry.Id = Guid.NewGuid().ToString();
            if (entry.CollectionDate == DateTime.Today) entry.Status = "Available";
            else entry.Status = "Requested";

            entries.Add(entry);

            return CreatedAtAction(nameof(GetEntry), new { id = entry.Id }, entry);
        }


        // update Entry
        [HttpPut("update/{id}")]
        public IActionResult UpdateEntry(string id, BloodBankEntry entry)
        {
            var res = entries.Find(i => i.Id == id);
            if (res == null) return NotFound();
            if (entry == null) return BadRequest("Invalid Attempt");

            if(entry.DonorName != null) res.DonorName = entry.DonorName;
            if(entry.Age != null && entry.Age >= 18) res.Age = entry.Age;
            if(entry.Status == "Available") res.Status = entry.Status;
            if(entry.CollectionDate != null && entry.CollectionDate > res.CollectionDate) 
                   res.CollectionDate = entry.CollectionDate;
            if (entry.CollectionDate == DateTime.Today) entry.Status = "Available";
            else entry.Status = "Requested";
            if (entry.ExpirationDate != null && entry.ExpirationDate != res.ExpirationDate)
                res.ExpirationDate = entry.ExpirationDate;

            return NoContent();
        }

        // Delete Entry
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteEntry(string id)
        {
            var res = entries.Find(i => i.Id == id);

            if (res == null) return NotFound();

            entries.Remove(res);

            return NoContent();
        }


        // pagination
        [HttpGet("filter")]
        public ActionResult<IEnumerable<BloodBankEntry>> Pagination(int page = 1, int size = 5)
        {
            var res = entries.Skip((page - 1) * size).Take(size).ToList();
            if (!res.Any()) return NotFound();
            return res;
        }

        // search
        [HttpGet("search")]
        public ActionResult<IEnumerable<BloodBankEntry>> Search(string? bloodType, string? status, string? donorName)
        {
            var res = entries.AsQueryable();

            if(!string.IsNullOrEmpty(bloodType)) res = res.Where(i => i.BloodType.Equals(bloodType, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(status)) res = res.Where(i => i.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            if(!string.IsNullOrEmpty(donorName)) res = res.Where(i => i.DonorName.Equals(donorName, StringComparison.OrdinalIgnoreCase));

            if (!res.Any()) return NotFound();
            return res.ToList();
        }
    }
}
