namespace Systems
{
    /*[DisableAutoCreation]
    public class PlayerInputClickSystem : ComponentSystem
    {
        private EntityManager _em;
        private GameState _gameState;
        
        protected override void OnCreate()
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Entity entity = _em.CreateEntity();
                _em.AddComponentData(entity, new PlayerInputClick());
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                if (HasSingleton<GameState>())
                {
                    _gameState = GetSingleton<GameState>();
                    if (_gameState.state == GameState.State.WaitingToStart)
                    {
                        _gameState.state = GameState.State.Playing;
                    }
                    else if(_gameState.state == GameState.State.Playing)
                    {
                        _gameState.state = GameState.State.Dead;
                    }
                    else
                    {
                        _gameState.state = GameState.State.WaitingToStart;
                    }
                    SetSingleton(_gameState);
                }
                
            }
        }
    }*/
}
