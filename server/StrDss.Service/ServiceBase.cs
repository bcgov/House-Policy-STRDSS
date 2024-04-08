using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
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
        private readonly ILogger<StrDssLogger> _logger;

        public ServiceBase(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger)
        {
            _currentUser = currentUser;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected string GetHostUrl()
        {
            var request = _httpContextAccessor?.HttpContext?.Request;
            return $"{request?.Scheme}://{request?.Host}";
        }
    }
}
