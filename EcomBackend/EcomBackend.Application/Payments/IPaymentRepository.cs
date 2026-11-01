using EcomBackend.Domain.Payments;

namespace EcomBackend.Application.Payments;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
}
