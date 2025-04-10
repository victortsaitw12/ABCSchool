using System;

namespace ABCSchool.Application.Wrappers;

public interface IResponseWrapper
{
    List<string> Messages { set; get; }
    bool IsSuccessful { set; get; }
}


public interface IResponseWrapper<out T> : IResponseWrapper
{
    T Data { get; }
}
