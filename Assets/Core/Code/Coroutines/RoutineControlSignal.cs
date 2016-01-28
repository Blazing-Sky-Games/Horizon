//base type of things that can be yilded from a routine
public class RoutineControlSignal{}

//base typeof things that a routine can break on
public class RoutineYieldInstruction : RoutineControlSignal{}

public class WaitForNextUpdate : RoutineYieldInstruction{}