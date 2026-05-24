using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Models;

namespace QuestionService.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(QuestionDbContext db) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Question>> CreateQuestions(CreateQuestionDto dto)
    {

        var validTags = await db.Tags.Where(a => dto.Tags.Contains(a.Slug)).ToListAsync();
        
        var invalidTags = dto.Tags.Except(validTags.Select(a => a.Slug).ToList()).ToList();

        if (invalidTags.Count != 0)
        {
            return BadRequest($"Invalid tags: {string.Join(", ", invalidTags)}");
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue("name");

        if (userId is null || name is null)
        {
            return BadRequest("Cannot get user id or name");
        }

        var question = new Question
        {
            Title = dto.Title,
            Content = dto.Content,
            AskerId = userId,
            AskerDisplayName = name,
            TagSlugs = dto.Tags,
        };
        
        db.Questions.Add(question);
        await db.SaveChangesAsync();
        
        return Created($"/questions/{question.Id}", question);

    }
    
    [HttpGet]
    public async Task<ActionResult<List<Question>>> GetQuestions(string? tag)
    {
        var query = db.Questions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(a => a.TagSlugs.Contains(tag));
        }
        
        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(string id)
    {
        var question = await db.Questions.FindAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        await db.Questions.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => 
                setters.SetProperty(a => a.ViewCount, a => a.ViewCount + 1));
        
        return question;
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateQuestion(string id, CreateQuestionDto dto)
    {
        var question = await db.Questions.FindAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId !=question.AskerId)
        {
            return Forbid();
        }
        
        var validTags = await db.Tags.Where(a => dto.Tags.Contains(a.Slug)).ToListAsync();
        
        var invalidTags = dto.Tags.Except(validTags.Select(a => a.Slug).ToList()).ToList();

        if (invalidTags.Count != 0)
        {
            return BadRequest($"Invalid tags: {string.Join(", ", invalidTags)}");
        }
        
        question.Title = dto.Title;
        question.Content = dto.Content;
        question.TagSlugs = dto.Tags;
        question.UpdatedAt = DateTime.UtcNow;
        
        await db.SaveChangesAsync();
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteQuestion(string id)
    {
        var question = await db.Questions.FindAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId !=question.AskerId)
        {
            return Forbid();
        }
        
        db.Questions.Remove(question);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}