using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineJudge.Models;
using System.Diagnostics;

namespace OnlineJudge.Controllers
{
    public class SubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Submissions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Submissions.ToListAsync());
        }

        // GET: Submissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (submission == null) return NotFound();

            return View(submission);
        }

        // GET: Submissions/Create
        public IActionResult Create(int problemId)
        {
            return View(new Submission
            {
                ProblemId = problemId
            });
        }

        // POST: Submissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Submission submission)
        {
            if (!ModelState.IsValid)
                return View(submission);

            submission.CreatedAt = DateTime.Now;

            var tests = _context.TestCases
                .Where(t => t.ProblemId == submission.ProblemId)
                .ToList();

            int testNumber = 1;

            foreach (var test in tests)
            {
                var result = RunProgram(submission.Code, test.Input);

                if (result == "CE")
                {
                    submission.Result = "Compilation Error";
                    break;
                }

                if (result == "TLE")
                {
                    submission.Result = $"Time Limit Exceeded on test {testNumber}";
                    break;
                }

                if (result.Trim() != test.Output.Trim())
                {
                    submission.Result = $"Wrong Answer on test {testNumber}";
                    break;
                }

                testNumber++;
            }

            if (string.IsNullOrEmpty(submission.Result))
            {
                submission.Result = "Accepted";
            }

            _context.Add(submission);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        string RunProgram(string code, string input)
        {
            string codePath = "/tmp/main.cpp";
            string exePath = "/tmp/main";

            System.IO.File.WriteAllText(codePath, code);

            var compile = Process.Start(new ProcessStartInfo
            {
                FileName = "g++",
                Arguments = $"{codePath} -o {exePath}",
                RedirectStandardError = true,
                UseShellExecute = false
            });

            compile.WaitForExit();

            if (compile.ExitCode != 0)
            {
                return "CE";
            }

            var run = new ProcessStartInfo
            {
                FileName = exePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(run);

            process.StandardInput.Write(input);
            process.StandardInput.Close();

            bool finished = process.WaitForExit(2000);

            if (!finished)
            {
                process.Kill();
                return "TLE";
            }

            string output = process.StandardOutput.ReadToEnd();

            return output;
        }

        // GET: Submissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var submission = await _context.Submissions.FindAsync(id);

            if (submission == null) return NotFound();

            return View(submission);
        }

        // POST: Submissions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Submission submission)
        {
            if (id != submission.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(submission);

            try
            {
                _context.Update(submission);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Submissions.Any(e => e.Id == submission.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Submissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (submission == null) return NotFound();

            return View(submission);
        }

        // POST: Submissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);

            if (submission != null)
            {
                _context.Submissions.Remove(submission);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
