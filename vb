package com.example.dbcruise.service;

import com.example.dbcruise.dto.PostResponse;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Service
public class DbCruiseService {
    private final RestTemplate restTemplate;
    private final ObjectMapper objectMapper;

    @Value("${dbcruise.service.url}")
    private String serviceUrl;

    public DbCruiseService(RestTemplate restTemplate, ObjectMapper objectMapper) {
        this.restTemplate = restTemplate;
        this.objectMapper = objectMapper;
    }

    public List<PostResponse> getFilteredPosts() {
        try {
            // Fetch raw JSON as a String
            String jsonResponse = restTemplate.getForObject(serviceUrl, String.class);

            // Debugging: Print the raw JSON response
            System.out.println("Raw JSON Response: " + jsonResponse);

            // Parse the JSON string into a list of PostResponse objects
            return objectMapper.readValue(jsonResponse, new TypeReference<List<PostResponse>>() {});
        } catch (Exception e) {
            throw new RuntimeException("Error fetching posts: " + e.getMessage(), e);
        }
    }
}
