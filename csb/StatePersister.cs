using System.Linq;

namespace CG.CodersStrikeBack
{
    public class StatePersister
    {
        private PodMove[] lastMyMoves;
        private PodMove[] lastHisMoves;
        private Pod[] lastMyPods;
        private Pod[] lastHisPods;

        public bool IsInitialState => lastMyPods == null;

        /// <summary>
        /// Заполняет в State те поля, которые не считываются из ввода, но важны для алгоритма. Это такая информация для каждой машинки:
        ///     * количество взятых чекпоинтов
        ///     * время с последнего взятого чекпоинта
        ///     * включён ли щит и сколько он ещё будет действовать
        ///     * есть ли ещё возможность буста
        /// 
        /// Для этого нужно вызывать FillState один раз на каждом ходу, передавая в него введенный state. 
        /// А потом вызывать RegisterMoves один раз на каждый ход, передавая ему известные ходы. 
        /// В игре известны только свои ходы. А при симуляции логов известны и ходы соперника.
        /// </summary>
        /// <param name="stateFromInput"></param>
        public void FillState(State stateFromInput)
        {
            if (!stateFromInput.IsInitialState)
            {
                // 1. Заполнить у всех машинок поля CheckpointsTaken и TimeWithoutCheckpoint исходя из предыдущего состояния машинок.
                // 2. Применить известные последние ходы к соответствующим последним машинкам и заполнить CanBoost и ShieldTicksLeft новых машинок из старых.
                // 3. Опционально, перед п.2 попытаться восстановить последние ходы врага, если они не известны 
                //    (они известны только в тесте на корректность симуляции)
                //    знание, какие у врага ещё есть абилки может пригодится при прогнозировании врага.
            }
            lastMyPods = stateFromInput.MyPods.Select(p => p.MakeCopy()).ToArray();
            lastHisPods = stateFromInput.HisPods.Select(p => p.MakeCopy()).ToArray();
            lastMyMoves = null;
            lastHisMoves = null;
        }
        public void RegisterMoves(PodMove[] myMoves, PodMove[] hisMoves = null)
        {
            lastMyMoves = myMoves;
            lastHisMoves = hisMoves;
        }
    }
}

