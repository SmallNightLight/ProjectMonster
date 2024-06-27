#ifndef UTILITY_INCLUDED
#define UTILITY_INCLUDED

void GetStep_float(float value, float step1, float step2, float step3, out float ramp) 
{
    int stepCount = 3;
    ramp = 0;

    //float stepVal = 1.0 / steps;
    
    for (int i = stepCount; i >= 0; i--)
    {
        float target = stepVal * i;
        
        if (value > target)
        {
            ramp = target;
            return;
        }
    }
}

#endif





























