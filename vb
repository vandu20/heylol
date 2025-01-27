package com.example.dbcruise.controller;

import com.example.dbcruise.dto.PostResponse;
import com.example.dbcruise.service.DbCruiseService;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
public class DbCruiseController {
    private final DbCruiseService dbCruiseService;

    public DbCruiseController(DbCruiseService dbCruiseService) {
        this.dbCruiseService = dbCruiseService;
    }

    @GetMapping("/snapshot")
    public List<PostResponse> getFilteredPosts() {
        return dbCruiseService.getFilteredPosts();
    }
}
