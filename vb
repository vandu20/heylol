package com.example.dbcruise.dto;

import lombok.Data;

@Data // Lombok annotation to generate getters, setters, toString, equals, and hashCode
public class PostResponse {
    private int userId;
    private int id;
    private String title;
    private String body; // Keep this field to match the JSON structure
}
