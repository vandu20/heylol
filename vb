package com.example.dbcruise.dto;

import com.fasterxml.jackson.annotation.JsonIgnore;
import lombok.Data;

@Data
public class PostResponse {
    private int userId;
    private int id;
    private String title;

    @JsonIgnore // Ignore this field during serialization and deserialization
    private String body;
}
