﻿using Luyenthi.Core.Dtos;
using Luyenthi.Core.Enums;
using Luyenthi.Domain;
using Luyenthi.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Luyenthi.Services
{
    public class DocumentHistoryService
    {
        private readonly DocumentHistoryRepository _documentHistoryRepository;
        private readonly QuestionSetService _questionSetService;
        private readonly QuestionHistoryService _questionHistoryService;
        public DocumentHistoryService(
            DocumentHistoryRepository documentHistoryRepository,
            QuestionSetService questionSetService,
            QuestionHistoryService questionHistoryService
            ) 
        {
            _documentHistoryRepository = documentHistoryRepository;
            _questionSetService = questionSetService;
            _questionHistoryService = questionHistoryService;
        }
        public DocumentHistory Create(DocumentHistory documentHistory)
        {
            _documentHistoryRepository.Add(documentHistory);
            return documentHistory;
        }
        public DocumentHistory Update(DocumentHistory documentHistory)
        {
            var current = _documentHistoryRepository.Get(documentHistory.Id);
            current.NumberCorrect = documentHistory.NumberCorrect;
            current.EndTime = documentHistory.EndTime;
            current.NumberIncorrect = documentHistory.NumberIncorrect;
            current.TimeDuration = documentHistory.TimeDuration;
            current.Status = documentHistory.Status;
            _documentHistoryRepository.UpdateEntity(current);
            return current;
        }
        public DocumentHistory GetById(Guid id)
        {
            var history = _documentHistoryRepository.Get(id);
            return history;
        }
        public async Task<DocumentHistory> GetDetailByDocumentId(
            Guid userId, Guid? documentId, Guid? Id = null, DocumentHistoryStatus? status = null)
        {
            var documentHistory = await _documentHistoryRepository
                .Find(i => i.CreatedBy == userId && (i.DocumentId == documentId || i.Id == Id) 
                            && (status == null ||status == i.Status))
                .OrderByDescending(i => i.StartTime)
                .Take(1)
                .Select(h => new DocumentHistory
                {
                    Id = h.Id,
                    StartTime = h.StartTime,
                    EndTime = h.EndTime,
                    Status = h.Status,
                    DocumentId = h.DocumentId,
                    NumberCorrect = h.NumberCorrect,
                    NumberIncorrect = h.NumberIncorrect,
                    QuestionHistories = h.QuestionHistories.Select(q => new QuestionHistory { 
                        Id = q.Id, 
                        QuestionId = q.QuestionId,
                        QuestionSetId = q.QuestionSetId,
                        DocumentHistoryId=h.Id,
                        Answer = q.Answer,
                        AnswerStatus =q.AnswerStatus
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return documentHistory;
        }
        public async Task CloseHistory(DocumentHistory documentHistory,int times = 0)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            // kiểm tra đáp án
            documentHistory.EndTime = DateTime.Now <= documentHistory.StartTime.AddMinutes(times) || times == 0  ? DateTime.Now : documentHistory.StartTime.AddMinutes(times);
            documentHistory.Status = DocumentHistoryStatus.Close;
            var questionSets = await _questionSetService.GetByDocumentId((Guid)documentHistory.DocumentId);
            var questions = questionSets.SelectMany(qs => qs.Questions)
                .SelectMany(q => q.Type == QuestionType.QuestionGroup ? q.SubQuestions : new List<Question> { q });
            var questionHistories = documentHistory.QuestionHistories.Select(qh =>
            {
                var question = questions.FirstOrDefault(i => i.Id == qh.QuestionId);
                qh.AnswerStatus = QuestionHelper.CheckAnswer(question, qh);
                return qh;
            }).ToList();
            _questionHistoryService.UpdateMany(questionHistories);
            documentHistory.NumberCorrect = questionHistories.Count(i => i.AnswerStatus == AnswerStatus.Correct);
            documentHistory.NumberIncorrect = questionHistories.Count() - documentHistory.NumberCorrect;
            documentHistory.TimeDuration = (documentHistory.EndTime - documentHistory.StartTime).TotalMinutes;
            Update(documentHistory);
            scope.Complete();
            scope.Dispose();
            documentHistory.QuestionHistories = questionHistories.ToList();
        }
        public async Task<UserAnalyticResponse> GetAnalyticUser(UserAnalyticQuery query)
        {
            var result = new UserAnalyticResponse();
            var documentHistories = await _documentHistoryRepository
                .Find(i => i.Status == DocumentHistoryStatus.Close &&
                     (query.GradeCode == null || i.Document.Grade.Code == query.GradeCode) &&
                     (query.SubjectCode == null || i.Document.Subject.Code == query.SubjectCode) &&
                     (query.UserId == null || i.CreatedBy == query.UserId)
                )
                .GroupBy(i => i.DocumentId)
                .Select(h => new {
                    DocumentId = h.Key,
                    NumberDocument = h.Count(),
                    MaxScore = h.Max(s => (double)(s.NumberCorrect / (s.NumberCorrect + s.NumberIncorrect))),
                    Medium = h.Average(s => s.NumberCorrect / (s.NumberCorrect + s.NumberIncorrect)),
                    TotalTime = h.Sum(s => s.TimeDuration)
                }).ToListAsync();
            result.NumberDocument = documentHistories.Sum(i => i.NumberDocument);
            result.PercentCorrect = Math.Round((double)documentHistories.Average(i => i.Medium) * 100, 2);
            result.TotalTime = documentHistories.Sum(i => i.TotalTime);
            result.Medium = documentHistories.Average(i => i.Medium);
            result.MaxScore = documentHistories.Max(i => Math.Round(i.MaxScore, 2));
            return result;
        }
        public async Task<List<UserHistoryAnalyticDto>> GetUserHistoryAnalytic(UserHistoryAnalyticQuery query)
        {
            var timeAnalytic = DocumentHelper.GetTimeAnalytic(query.Type);
            var histories = await _documentHistoryRepository
                .Find(i => (query.UserId == Guid.Empty || i.CreatedBy == query.UserId) &&
                      i.Status == DocumentHistoryStatus.Close &&
                      i.EndTime >= timeAnalytic.StartTime && i.EndTime <= timeAnalytic.EndTime)
                .Select(i => new DocumentHistory
                {
                    Id = i.Id,
                    DocumentId = i.DocumentId,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    NumberCorrect = i.NumberCorrect,
                    NumberIncorrect = i.NumberIncorrect,
                    TimeDuration = i.TimeDuration
                })
                .ToListAsync();
            var historyAnalytics= histories
                .GroupBy(i => {
                    switch (query.Type)
                    {
                        case UserHistoryAnalyticType.Today:
                            return i.EndTime.Hour/2;
                        case UserHistoryAnalyticType.InWeek:
                            return i.EndTime.Day;
                        case UserHistoryAnalyticType.InMonth:
                            return i.EndTime.Day/3;
                        case UserHistoryAnalyticType.InYear:
                            return i.EndTime.Month;
                    };
                    return 1 ;
                })
                .Select(i => new UserHistoryAnalyticDto
                {
                    Key=i.Key,
                    Label = DocumentHelper.GetLabelAnalytic(query.Type, i.Key),
                    MaxScore = Math.Round(i.Max(h => (double)h.NumberCorrect / (h.NumberCorrect + h.NumberIncorrect)*10),2),
                    Total = i.Count(),
                    TimeDuration = i.Sum(h => h.TimeDuration),
                    Medium = Math.Round(i.Average(h => (double)h.NumberCorrect / (h.NumberCorrect + h.NumberIncorrect)) * 10,2),
                    StartDate = i.Min(i => i.EndTime),
                    EndDate = i.Max(i => i.EndTime)
                    
                }).ToList();
            // lấp đầy dữ liệu
            var StartTime = timeAnalytic.StartTime;
            var EndTime = timeAnalytic.StartTime;
            var results = new List<UserHistoryAnalyticDto>();
            while(StartTime <= timeAnalytic.EndTime) {
                var key = 1;
                switch (query.Type)
                {
                    case UserHistoryAnalyticType.Today:
                        key= StartTime.Hour / 2;
                        EndTime = EndTime.AddHours(2);
                        break;
                    case UserHistoryAnalyticType.InWeek:
                        key = StartTime.Day ;
                        EndTime = EndTime.AddDays(1);
                        break;
                    case UserHistoryAnalyticType.InMonth:
                        key = StartTime.Day/3;
                        EndTime = EndTime.AddDays(3);
                        break;
                    case UserHistoryAnalyticType.InYear:
                        key = StartTime.Month ;
                        EndTime = EndTime.AddMonths(1);
                        break;
                };
                var userHistory = historyAnalytics.Find(i => i.Key == key);
                if(userHistory == null)
                {
                    results.Add(new UserHistoryAnalyticDto
                    {
                        Key=key,
                        Label=DocumentHelper.GetLabelAnalytic(query.Type, key),
                        TimeDuration=0,
                        StartDate = StartTime,
                        EndDate = EndTime,
                        Medium=0,
                        MaxScore = 0,
                        Total = 0
                    });
                }
                else
                {
                    results.Add(userHistory);
                }
                StartTime = EndTime;
            }
            
            return results.OrderBy(i => i.StartDate).ToList();
        }
        public async Task<List<UserDocumentAnalyticDto>> GetRanks()
        {
            return new List<UserDocumentAnalyticDto>();
        }
    }
}
