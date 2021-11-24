using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionManager;
using TransactionManager.Models;

namespace TransactionManager.Controllers
{
    [Route("api/v1/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/transactions
        [HttpGet]
        public IEnumerable<Transaction> GetTransactions()
        {
            return _context.Transactions;
        }

        // GET: api/v1/transactions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // GET: api/v1/transactions/invoices/2021-11-23/2021-11-24
        [HttpGet("invoices/{firstDate}/{secondDate}")]
        public async Task<IActionResult> GetInvoicesByDateRange([FromRoute] DateTime firstDate, DateTime secondDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transactions = await _context.Transactions.Where(t => t.Date >= firstDate && t.Date <= secondDate).ToListAsync();

            if (transactions == null)
            {
                return NotFound();
            }
            else
            {
                // Mark un-billed transactions as billed and update records
                for(int i = 0; i<= transactions.Count - 1 ; i++)
                {
                    if(transactions[i].PaymentStatus == "Un-billed")
                    {
                        transactions[i].PaymentStatus = "Billed";

                        _context.Entry(transactions[i]).State = EntityState.Modified;

                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!TransactionExists(transactions[i].Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            return Ok(transactions);
        }

        // PUT: api/v1/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction([FromRoute] int id, [FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Record updated");
        }

        // POST: api/v1/transactions
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        // DELETE: api/v1/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        // POST: api/v1/transactions/pay/1
        [HttpPost("pay/{id}")]
        public async Task<IActionResult> PayTransaction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = new Transaction()
            {
                Id = id,
                PaymentStatus = "Paid"
            };

            // Set payment status as paid and update the record 
            _context.Transactions.Attach(transaction);
            _context.Entry(transaction).Property(x => x.PaymentStatus).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Transaction paid");
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}