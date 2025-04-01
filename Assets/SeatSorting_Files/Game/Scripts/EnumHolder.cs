namespace EKStudio
{
    using UnityEngine;
    
    
    public enum BusSeatPassType
    {
        None,
        Blue,
        Red,
        Green,
        Orange,
        Pink,
        Purple,
        Yellow,
        Gray,
        SpringGreen
    
    }
    
    public enum SeatState
    {
        OnStaying,
        OnMoving
    }
    
    public enum BusState
    {
        OnStaying,
        OnMovingToSlot,
        OnParking,
        OnMovingToOut
    }
}
