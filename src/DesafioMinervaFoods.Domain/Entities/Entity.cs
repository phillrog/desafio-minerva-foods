namespace DesafioMinervaFoods.Domain.Entities
{
    public abstract class EntityBase<TId>
    {
        public TId Id { get; protected set; } = default!;
    }
    public abstract class Entity<TId> : EntityBase<TId>
    {
        // Criação
        public DateTime CreatedAt { get; internal set; }
        public Guid CreatedBy { get; internal set; } // ID do usuário ou sistema

        // Alteração
        public DateTime? UpdatedAt { get; internal set; }
        public Guid? UpdatedBy { get; internal set; }

        public void CriadoPorUsuario(Guid userId)
        {
            CreatedBy = userId;
        }

        public void AlteradoPorUsuario(Guid userId)
        {
            UpdatedBy = userId;
        }
    }
}
