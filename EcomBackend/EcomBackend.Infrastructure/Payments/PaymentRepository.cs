using EcomBackend.Application.Payments;
using EcomBackend.Domain.Payments;
using EcomBackend.Infrastructure.Data;
                               
namespace EcomBackend.Infrastructure.Payments;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
