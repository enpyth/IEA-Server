
![1](./imgs/class_diagram.svg)

```
classDiagram
    class User {
        +UUID user_id
        +String email
        +String role
        +view_content()
    }

    class Visitor {
    }

    class AuthenticatedUser {
    }

    class Enterprise {
        +String company_name
        +request_collaboration(expert, details)
    }

    class Expert {
        +String expertise_area
        +request_collaboration(enterprise, details)
    }

    class Admin {
        +verify_user(user, target_role)
    }

    class CollaborationRequest {
        +UUID id
        +User sender
        +User receiver
        +String details
        +String status
        +approve()
        +reject()
    }

    class AcademicProducts {
        +UUID id
        +UUID expert_id
        +JSONB achievements
    }

    User <|-- Visitor
    User <|-- AuthenticatedUser
    User <|-- Enterprise
    User <|-- Expert
    User <|-- Admin

    Enterprise --> CollaborationRequest : "initiates"
    Expert --> CollaborationRequest : "initiates"
    CollaborationRequest --> User : "sender"
    CollaborationRequest --> User : "receiver"

    Expert --> AcademicProducts : "owns *"
```