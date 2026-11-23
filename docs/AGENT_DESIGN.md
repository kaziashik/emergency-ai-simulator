# AGENT DESIGN

## 1. Risk Assessment Agent

Input:
- Fire location
- Fire severity
- Number of occupants
- Exit availability

Responsibilities:
- Analyse emergency conditions.
- Assign a simulated risk level.
- Explain the risk assessment.

Output:
- Risk level
- Identified hazards
- Explanation

## 2. Evacuation Planning Agent

Input:
- Risk assessment
- Available exits
- Occupant locations
- Blocked routes

Responsibilities:
- Evaluate available routes.
- Recommend an evacuation route.
- Explain the recommendation.

Output:
- Recommended exit
- Route
- Reasoning

## 3. Coordination Agent

Input:
- Risk assessment result
- Evacuation planning result

Responsibilities:
- Combine agent outputs.
- Identify conflicting recommendations.
- Prepare a final recommendation.

Output:
- Final recommendation
- Supporting information
- Uncertainty or limitations

## 4. Human Operator

Responsibilities:
- Review recommendations.
- Approve or reject proposed actions.
- Record decisions.

## Safety

AI recommendations must be validated.
The AI cannot directly execute consequential
emergency actions without operator approval.
