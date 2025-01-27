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
            // Adjusted filter format, ensuring correct syntax
            String jsonRequestBody = "{\"filter\": \"exp = in(userId, 4)\"}"; // Correct the request body structure

            // Set up HTTP headers (if needed)
            HttpHeaders headers = new HttpHeaders();
            headers.set("Content-Type", "application/json");

            // Create HttpEntity with body and headers
            HttpEntity<String> entity = new HttpEntity<>(jsonRequestBody, headers);

            // Send the POST request with the filter
            ResponseEntity<String> response = restTemplate.exchange(serviceUrl, HttpMethod.POST, entity, String.class);

            // Print the raw JSON response
            System.out.println("Raw JSON Response: " + response.getBody());

            // Debugging: Check if the response matches expected format
            // Inspect the raw response before deserialization
            if (response.getBody() != null) {
                // Try deserializing manually into raw JSON first to understand the structure
                String rawJson = response.getBody();
                System.out.println("Raw JSON Received: " + rawJson);

                // Try deserializing as a map or list based on actual response structure
                List<PostResponse> posts = objectMapper.readValue(rawJson, objectMapper.getTypeFactory().constructCollectionType(List.class, PostResponse.class));
                return posts;
            }

            return null;

        } catch (HttpClientErrorException e) {
            throw new RuntimeException("Error in API call: " + e.getMessage(), e);
        } catch (Exception e) {
            throw new RuntimeException("Error fetching posts: " + e.getMessage(), e);
        }
    }
}
