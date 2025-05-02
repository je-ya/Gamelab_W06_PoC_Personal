public interface IState
{
    void Enter(StateConfig config); //상태 진입시 실행할 동작


    void Execute(); //해당 상태에서 실행할 동작


    void Exit();    //상태 종료 조건 및 다음 상태로 전환

}
