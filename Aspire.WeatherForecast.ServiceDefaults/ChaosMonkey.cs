namespace Aspire.WeatherForecast.ServiceDefaults;

public static class ChaosMonkey
{
    public static Chaos NoChaos()
    {
        return new Chaos(false, 0);
    }

    public static Chaos Create()
    {
        var random = Random.Shared.Next(1, 15);
        var delayInSeconds = 0;
        var chaosCreated = false;

        // Randomly throw exception
        if(random % 5 == 0)
        {   
            throw new ChaosMonkeyException("Greetings from the Chaos Monkey!");
        }
        
        // Randomly delay execution
        if(random % 3 == 0)
        {  
            chaosCreated = true;
            delayInSeconds = random; 
            Thread.Sleep(delayInSeconds * 1000);
        }

        return new Chaos(chaosCreated, delayInSeconds);
    }
}

public readonly struct Chaos(bool created, int delayInSeconds)
{
    public bool Created { get; } = created;
    public int DelayInSeconds { get; } = delayInSeconds;
}

public class ChaosMonkeyException(string? message) : Exception(message)
{
}