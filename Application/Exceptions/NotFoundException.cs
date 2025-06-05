namespace Application.Exceptions;

public class NotFoundException(object key) : Exception($"Resource with key {key} was not found.");
public class BadRequestException(object key) : Exception ($"{key}");