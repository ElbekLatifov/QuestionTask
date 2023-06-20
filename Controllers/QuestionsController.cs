﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionTask.Managers;
using QuestionTask.Models;

namespace QuestionTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly QuestionManager questionManager;

    public QuestionsController(QuestionManager questionManager)
    {
        this.questionManager = questionManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions()
    {
        var questions = await questionManager.GetQuestions();
        return Ok(questions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuestionModel model)
    {
        var question = await questionManager.CreateQuestion(model); 
        return Ok(question);
    }

    [HttpPost("/addphotoToQuestion")]
    public async Task<IActionResult> AddPhoto([FromForm] Media photo)
    {
        var path = await questionManager.AddPhotoToQuestion(photo.QuestionId, photo.Photo);
        return Ok(path);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var question = await questionManager.GetQuestionById(id);
        return Ok(question);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] QuestionModel model)
    {
        var question = await questionManager.UpdateQuestion(id, model);
        return Ok(question);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteQuestion(Guid id)
    {
        await questionManager.DeleteQuestion(id);
        return Ok("Deleted");
    }
}
