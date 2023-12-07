namespace Systems
{
    /*[DisableAutoCreation]
    public class RenderTileSystem : ComponentSystem
    {
        private EntityManager _em;
        
        protected override void OnCreate()
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            if (HasSingleton<DataCore>())
            {
                var data = GetSingleton<DataCore>();
                Entities.ForEach((Entity entity, ref RenderTile render) =>
                {
                    _em.RemoveComponent<RenderTile>(entity);
                    _em.Instantiate(data.Tile1);
                }); 
            }
            
        }
    }*/
}
