﻿using System;
using System.Net;
using System.Threading.Tasks;
using JobManagmentSystem.Application.Common.Exceptions;
using JobManagmentSystem.Scheduler.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace JobManagmentSystem.WebApi.Common
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = string.Empty;

            code = exception switch
            {
                NotFoundException _ => HttpStatusCode.NotFound,
                WrongTaskNameBadRequestException _ => HttpStatusCode.BadRequest,
                WrongKeyBadRequestException _ => HttpStatusCode.BadRequest,
                _ => code
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;

            if (result == string.Empty)
            {
                result = JsonConvert.SerializeObject(new {error = exception.Message});
            }

            return context.Response.WriteAsync(result);
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}