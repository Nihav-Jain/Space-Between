using System.Collections;

public class KeyPressDelay {

    public bool isDelayed { get; private set; }
    public bool justAdded { get; private set; }

    public KeyPressDelay()
    {
        this.isDelayed = false;
        this.justAdded = false;
    }

    public void Enqueue()
    {
        this.isDelayed = true;
        this.justAdded = false;
    }

    public void PrepareForDequeue()
    {
        this.justAdded = false;
    }

    public void Dequeue()
    {
        this.isDelayed = false;
    }

    public bool HasBeenDequeued()
    {
        return !(this.isDelayed || this.justAdded);
    }
}
