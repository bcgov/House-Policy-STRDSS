using AutoMapper;
using Microsoft.AspNetCore.Http;
using StrDss.Data;
using StrDss.Model;

namespace StrDss.Service
{
    public class ServiceBase
    {
        protected ICurrentUser _currentUser;
        protected IFieldValidatorService _validator;
        protected IUnitOfWork _unitOfWork;
        protected IMapper _mapper;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceBase(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _currentUser = currentUser;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetHostUrl()
        {
            var request = _httpContextAccessor?.HttpContext?.Request;
            return $"{request?.Scheme}://{request?.Host}";
        }
    }
}
