package com.example.dbcruise.service;

import com.example.dbcruise.dto.PostResponse;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;

import java.util.List;
import java.util.Map;

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
            // Build request body
            String jsonRequestBody = "{\"userid\": [\"4\"]}"; // Replace with the desired filter

            // Set up HTTP headers (if needed)
            HttpHeaders headers = new HttpHeaders();
            headers.set("Content-Type", "application/json");

            // Create HttpEntity with body and headers
            HttpEntity<String> entity = new HttpEntity<>(jsonRequestBody, headers);

            // Send the POST request with the filter
            ResponseEntity<String> response = restTemplate.exchange(serviceUrl, HttpMethod.POST, entity, String.class);

            // Debugging: Print the raw JSON response
            System.out.println("Raw JSON Response: " + response.getBody());

            // Parse the response into PostResponse objects
            List<PostResponse> posts = objectMapper.readValue(response.getBody(), objectMapper.getTypeFactory().constructCollectionType(List.class, PostResponse.class));
            return posts;

        } catch (HttpClientErrorException e) {
            throw new RuntimeException("Error in API call: " + e.getMessage(), e);
        } catch (Exception e) {
            throw new RuntimeException("Error fetching posts: " + e.getMessage(), e);
        }
    }
}
